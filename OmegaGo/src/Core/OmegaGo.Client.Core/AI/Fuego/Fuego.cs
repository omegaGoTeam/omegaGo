﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.AI.FuegoSpace
{
    /// <summary>
    ///     The Fuego AI is a Monte Carlo advanced Go intelligence.
    /// </summary>
    /// <seealso cref="AIProgramBase" />
    public class Fuego : AIProgramBase
    {
        private const float ComparisonTolerance = 0.00001f;

        private readonly List<Move> _history = new List<Move>();
        private readonly List<string> _storedNotes = new List<string>();
        private IGtpEngine _engine;

        private bool _initialized;
        private int _timelimit = -1;
        private bool SendAllAiOutputToLog = true;
        private bool SendDebuggingInformationToLogToo = true;

        /// <summary>
        ///     Capabilities of the AI
        /// </summary>
        public override AICapabilities Capabilities => new AICapabilities(false, true, 2, 19, true);

        /// <summary>
        ///     Allows resigning
        /// </summary>
        public bool AllowResign { get; set; }

        /// <summary>
        ///     Maximum number of games
        /// </summary>
        public int MaxGames { get; set; }

        /// <summary>
        ///     Pondering
        /// </summary>
        public bool Ponder { get; set; }

        /// <summary>
        ///     Requests a move from Fuego AI
        /// </summary>
        /// <param name="gameInformation">Information about the requested move</param>
        /// <returns>Decision</returns>
        public override AIDecision RequestMove(AiGameInformation gameInformation)
        {
            FixHistory(gameInformation);

            // Move for what color?
            string movecolor = gameInformation.AIColor == StoneColor.Black ? "B" : "W";

            // Update remaining time
            var timeLeftArguments = gameInformation.AiPlayer.Clock.GetGtpTimeLeftCommandArguments();
            if (timeLeftArguments != null)
            {
                int secondsRemaining = timeLeftArguments.NumberOfSecondsRemaining;
                secondsRemaining = Math.Max(secondsRemaining - 2, 0);
                    // let's give the AI less time to ensure it does its move on time
                SendCommand("time_left " + movecolor + " " + secondsRemaining + " " +
                            timeLeftArguments.NumberOfStonesRemaining);
            }

            // Generate the next move
            string result = SendCommand("genmove " + movecolor).Text;
            if (result == "resign")
            {
                var resignDecision = AIDecision.Resign("Resigned because of low win chance.");
                resignDecision.AiNotes = this._storedNotes;
                this._storedNotes.Clear();
                return resignDecision;
            }
            var move = result == "PASS"
                ? Move.Pass(gameInformation.AIColor)
                : Move.PlaceStone(gameInformation.AIColor, Position.FromIgsCoordinates(result));

            // Change history
            this._history.Add(move);

            // Get win percentage
            string commandResult = SendCommand("uct_value_black").Text;
            float value = float.Parse(commandResult, CultureInfo.InvariantCulture);
            if (gameInformation.AIColor == StoneColor.White)
            {
                value = 1 - value;
            }
            string winChanceNote = (Math.Abs(value) < Fuego.ComparisonTolerance) ||
                                   (Math.Abs(value - 1) < Fuego.ComparisonTolerance)
                ? "Reading from opening book."
                : "Win chance (" + gameInformation.AIColor + "): " + 100*value + "%";
            Note(winChanceNote);
            var moveDecision = AIDecision.MakeMove(
                move, winChanceNote);
            moveDecision.AiNotes = this._storedNotes.ToList(); // copy

            // Prepare the way
            this._storedNotes.Clear();

            // Return result
            return moveDecision;
        }

        public GtpResponse SendCommand(string command)
        {
            var output = this._engine.SendCommand(command);
            if (this.SendAllAiOutputToLog)
            {
                Note(">" + command);
                Note(output.ToString());
            }
            return output;
        }

        public override AIDecision GetHint(AiGameInformation gameInformation)
        {
            var result = RequestMove(gameInformation);
            UndoOneMove();
            return result;
        }

        public override void MoveUndone()
        {
            UndoOneMove();
        }

        public override void MovePerformed(Move move, GameTree gameTree, GamePlayer informedPlayer, GameInfo info)
        {
            FixHistory(new AiGameInformation(info, informedPlayer.Info.Color, informedPlayer, gameTree));
        }

        /// <summary>
        ///     Gets all positions that the Fuego engines consider dead in its current state (as arrived at by its own moves,
        ///     RequestMoves calls
        ///     and MovePerformed/MoveUndone calls. Currently this is not multithreaded for ease of debugging.
        /// </summary>
        /// <returns></returns>
        public override async Task<IEnumerable<Position>> GetDeadPositions()
        {
            var result = SendCommand("final_status_list dead");

            var positions = result.Text.Split(new[] {' ', '\n'}, StringSplitOptions.RemoveEmptyEntries);
            var mark = new List<Position>();
            foreach (string position in positions)
            {
                mark.Add(Position.FromIgsCoordinates(position));
            }
            return mark;
        }

        private void FixHistory(AiGameInformation aiGameInformation)
        {
            // Initialize if not yet.
            if (!this._initialized)
            {
                Initialize(aiGameInformation);
                this._initialized = true;
            }

            // Fix history.
            var trueHistory = aiGameInformation.GameTree.PrimaryMoveTimeline.ToList();
            for (int i = 0; i < trueHistory.Count; i++)
            {
                if (this._history.Count == i)
                {
                    var trueMove = trueHistory[i];
                    this._history.Add(trueMove);
                    SendCommand("play " + (trueMove.WhoMoves == StoneColor.Black ? "B" : "W") + " " +
                                trueMove.Coordinates.ToIgsCoordinates());
                }
            }
        }

        private void Initialize(AiGameInformation gameInformation)
        {
            this._engine = AISystems.FuegoBuilder.CreateEngine(gameInformation.GameInfo.BoardSize.Width);

            // Board size
            SendCommand("boardsize " + gameInformation.GameInfo.BoardSize.Width);

            // Strength
            SendCommand("uct_param_player ponder " + (this.Ponder ? "1" : "0"));

            // Rules
            switch (gameInformation.GameInfo.RulesetType)
            {
                case RulesetType.AGA:
                case RulesetType.Chinese:
                    SendCommand("go_rules chinese");
                    SendCommand("go_param_rules japanese_scoring 0");
                    break;
                case RulesetType.Japanese:
                    SendCommand("go_rules japanese");
                    SendCommand("go_param_rules japanese_scoring 1");
                    break;
            }
            SendCommand("komi " + gameInformation.GameInfo.Komi.ToString(CultureInfo.InvariantCulture));
            if (this.MaxGames > 0)
            {
                SendCommand("uct_param_player max_games " + this.MaxGames);
            }

            if (!this.AllowResign)
            {
                SendCommand("uct_param_player resign_threshold 0");
            }

            // TODO Petr: on IGS, make it so two passes don't end a game

            // Time settings
            string timeSettings = gameInformation.AiPlayer.Clock.GetGtpInitializationCommand();
            if (timeSettings != null)
            {
                SendCommand(timeSettings);
            }

            // Regardless of time controls, we are never willing to wait more than 15 seconds.
            SendCommand("go_param timelimit 15");

            // Print beginning info
            Note("Komi set to " + SendCommand("get_komi").Text);
            DebuggingNote("Random seed is " + SendCommand("get_random_seed").Text);
            SendCommand("go_param_rules");
        }

        private void Note(string note)
        {
            this._storedNotes.Add(note);
        }

        private void DebuggingNote(string note)
        {
            if (this.SendDebuggingInformationToLogToo)
            {
                this._storedNotes.Add(note);
            }
        }

        private void UndoOneMove()
        {
            SendCommand("undo");
            this._history.RemoveAt(this._history.Count - 1);
        }
    }
}