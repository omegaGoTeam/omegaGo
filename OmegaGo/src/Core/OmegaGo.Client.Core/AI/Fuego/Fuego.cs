using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.AI.FuegoSpace
{
    /// <summary>
    /// The Fuego AI is a Monte Carlo advanced Go intelligence.
    /// </summary>
    /// <seealso cref="AIProgramBase" />
    public class Fuego : AIProgramBase
    {
        private bool SendAllAiOutputToLog = true;
        private bool SendDebuggingInformationToLogToo = true;

        private const float ComparisonTolerance = 0.00001f;

        private readonly List<Move> _history = new List<Move>();
        private readonly List<string> _storedNotes = new List<string>();

        private bool _initialized;
        private IGtpEngine _engine;
        private int _timelimit = -1;

        /// <summary>
        /// Capabilities of the AI
        /// </summary>
        public override AICapabilities Capabilities => new AICapabilities(false, true, 2, 19, true);

        /// <summary>
        /// Allows resigning
        /// </summary>
        public bool AllowResign { get; set; }

        /// <summary>
        /// Maximum number of games
        /// </summary>
        public int MaxGames { get; set; }

        /// <summary>
        /// Pondering
        /// </summary>
        public bool Ponder { get; set; }

        /// <summary>
        /// Requests a move from Fuego AI
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
                secondsRemaining = Math.Max(secondsRemaining - 2, 0); // let's give the AI less time to ensure it does its move on time
                SendCommand("time_left " + movecolor + " " + secondsRemaining + " " + timeLeftArguments.NumberOfStonesRemaining);
            }

            // Generate the next move
            string result = SendCommand("genmove " + movecolor).Text;
            if (result == "resign")
            {
                AIDecision resignDecision = AIDecision.Resign("Resigned because of low win chance.");
                resignDecision.AiNotes = _storedNotes;
                _storedNotes.Clear();
                return resignDecision;
            }
            var move = result == "PASS"
                ? Move.Pass(gameInformation.AIColor)
                : Move.PlaceStone(gameInformation.AIColor, Position.FromIgsCoordinates(result));

            // Change history
            _history.Add(move);

            // Get win percentage
            string commandResult = SendCommand("uct_value_black").Text;
            float value = float.Parse(commandResult, CultureInfo.InvariantCulture);            
            if (gameInformation.AIColor == StoneColor.White)
            {
                value = 1 - value;
            }
            string winChanceNote = (Math.Abs(value) < ComparisonTolerance) ||
                                   (Math.Abs(value - 1) < ComparisonTolerance)
                ? "Reading from opening book."
                : "Win chance (" + gameInformation.AIColor + "): " + (100*value) + "%";
            Note(winChanceNote);
            var moveDecision = AIDecision.MakeMove(
                move, winChanceNote);
            moveDecision.AiNotes = _storedNotes.ToList(); // copy

            // Prepare the way
            _storedNotes.Clear();

            // Return result
            return moveDecision;
        }

        private void FixHistory(AiGameInformation aiGameInformation)
        {
            // Initialize if not yet.
            if (!_initialized)
            {
                Initialize(aiGameInformation);
                _initialized = true;
            }

            // Fix history.
            var trueHistory = aiGameInformation.GameTree.PrimaryMoveTimeline.ToList();
            for (int i = 0; i < trueHistory.Count; i++)
            {
                if (this._history.Count == i)
                {
                    Move trueMove = trueHistory[i];
                    this._history.Add(trueMove);
                    SendCommand("play " + (trueMove.WhoMoves == StoneColor.Black ? "B" : "W") + " " +
                                trueMove.Coordinates.ToIgsCoordinates());
                }
            }
        }

        private void Initialize(AiGameInformation gameInformation)
        {
            _engine = AISystems.FuegoBuilder.CreateEngine(gameInformation.GameInfo.BoardSize.Width);

            // Board size
            SendCommand("boardsize " + gameInformation.GameInfo.BoardSize.Width);

            // Strength
            SendCommand("uct_param_player ponder " + (Ponder ? "1": "0"));

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
            if (MaxGames > 0)
            {
                SendCommand("uct_param_player max_games " + MaxGames);
            }
            
            if (!AllowResign)
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

        public GtpResponse SendCommand(string command)
        {
            GtpResponse output = _engine.SendCommand(command);
            if (SendAllAiOutputToLog)
            {
                Note(">" + command);
                Note(output.ToString());
            }
            return output;
        }
        
        private void Note(string note)
        {
            _storedNotes.Add(note);
        }

        private void DebuggingNote(string note)
        {
            if (SendDebuggingInformationToLogToo)
            {
                _storedNotes.Add(note);
            }
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
            FixHistory(new AI.AiGameInformation(info, informedPlayer.Info.Color, informedPlayer, gameTree));
        }

        private void UndoOneMove()
        {
            SendCommand("undo");
            _history.RemoveAt(_history.Count - 1);
        }
    }
}