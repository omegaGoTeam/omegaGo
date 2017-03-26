using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.AI.Fuego
{
    /// <summary>
    /// The Fuego AI is a Monte Carlo advanced Go intelligence.
    /// </summary>
    /// <seealso cref="AIProgramBase" />
    public class FuegoAI : AIProgramBase
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
        public override AICapabilities Capabilities => new AICapabilities(false, false, 2, 19, true);

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
        /// <param name="preMoveInformation">Information about the requested move</param>
        /// <returns>Decision</returns>
        public override AIDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            if (!_initialized)
            {
                Initialize(preMoveInformation);
                _initialized = true;
            }
            if (preMoveInformation.Difficulty != _timelimit)
            {
                SendCommand("go_param timelimit " + preMoveInformation.Difficulty);
                _timelimit = preMoveInformation.Difficulty;
            }
            var trueHistory = preMoveInformation.GameTree.PrimaryMoveTimeline.ToList();
            for (int i = 0; i < trueHistory.Count; i++)
            {
                if (_history.Count == i)
                {
                    Move trueMove = trueHistory[i];
                    _history.Add(trueMove);
                    SendCommand("play " + (trueMove.WhoMoves == StoneColor.Black ? "B" : "W") + " " +
                                           trueMove.Coordinates.ToIgsCoordinates());
                }
            }
            string movecolor = preMoveInformation.AIColor == StoneColor.Black ? "B" : "W";
            var timeLeftArguments = preMoveInformation.AiPlayer.Clock.GetGtpTimeLeftCommandArguments();
            if (timeLeftArguments != null)
            {
                int secondsRemaining = timeLeftArguments.NumberOfSecondsRemaining;
                secondsRemaining = Math.Max(secondsRemaining - 2, 0); // let's give the AI less time to ensure it does its move on time
                SendCommand("time_left " + movecolor + " " + secondsRemaining + " " + timeLeftArguments.NumberOfStonesRemaining);
            }
            string result = SendCommand("genmove " + movecolor).Text;
            if (result == "resign")
            {
                AIDecision resignDecision = AIDecision.Resign("Resigned because of low win chance.");
                resignDecision.AiNotes = _storedNotes;
                _storedNotes.Clear();
                return resignDecision;
            }
            var move = result == "PASS"
                ? Move.Pass(preMoveInformation.AIColor)
                : Move.PlaceStone(preMoveInformation.AIColor, Position.FromIgsCoordinates(result));
            _history.Add(move);
            string commandResult = SendCommand("uct_value_black").Text;
            float value = float.Parse(commandResult, CultureInfo.InvariantCulture);            
            if (preMoveInformation.AIColor == StoneColor.White)
            {
                value = 1 - value;
            }
            string winChanceNote = (Math.Abs(value) < ComparisonTolerance) ||
                                   (Math.Abs(value - 1) < ComparisonTolerance)
                ? "Reading from opening book."
                : "Win chance (" + preMoveInformation.AIColor + "): " + (100*value) + "%";
            Note(winChanceNote);
            var moveDecision = AIDecision.MakeMove(
                move, winChanceNote);
            moveDecision.AiNotes = _storedNotes;
            _storedNotes.Clear();
            return moveDecision;
        }

        private void Initialize(AIPreMoveInformation preMoveInformation)
        {
            _engine = AISystems.FuegoBuilder.CreateEngine(preMoveInformation.GameInfo.BoardSize.Width);

            // Board size
            SendCommand("boardsize " + preMoveInformation.GameInfo.BoardSize.Width);

            // Strength
            SendCommand("uct_param_player ponder " + (Ponder ? "1": "0"));

            // Rules
            switch (preMoveInformation.GameInfo.RulesetType)
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
            SendCommand("komi " + preMoveInformation.GameInfo.Komi.ToString(CultureInfo.InvariantCulture));
            if (MaxGames > 0)
            {
                SendCommand("uct_param_player max_games " + MaxGames);
            }
            
            if (!AllowResign)
            {
                SendCommand("uct_param_player resign_threshold 0");
            }
            
            // TODO Petr: send commands for allowResign, maxgames
            // TODO Petr: on IGS, make it so two passes don't end a game

            // Time settings
            string timeSettings = preMoveInformation.AiPlayer.Clock.GetGtpInitializationCommand();
            if (timeSettings != null)
            {
                SendCommand(timeSettings);
            }

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
    }
}