using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Fuego
{
    /// <summary>
    /// The Fuego AI is a Monte Carlo advanced Go intelligence.
    /// </summary>
    /// <seealso cref="AIProgramBase" />
    internal class FuegoAI : AIProgramBase
    {
        private const float ComparisonTolerance = 0.00001f;

        private readonly List<Move> _history = new List<Move>();

        private bool _initialized;
        private IGtpEngine _engine;
        private int _timelimit = -1;

        /// <summary>
        /// Capabilities of the AI
        /// </summary>
        public override AICapabilities Capabilities => new AICapabilities(false, false, 2, 19);

        /// <summary>
        /// Requests a move from Fuego AI
        /// </summary>
        /// <param name="preMoveInformation">Information about the requested move</param>
        /// <returns>Decision</returns>
        public override AIDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            if (!_initialized)
            {
                _engine = AISystems.FuegoBuilder.CreateEngine(preMoveInformation.GameInfo.BoardSize.Width);
                _engine.SendCommand("uct_param_player ponder 1");
                // TODO komi
                _initialized = true;
            }
            if (preMoveInformation.Difficulty != _timelimit)
            {
                _engine.SendCommand("go_param timelimit " + preMoveInformation.Difficulty);
                _timelimit = preMoveInformation.Difficulty;
            }
            var trueHistory = preMoveInformation.GameTree.PrimaryMoveTimeline.ToList();
            for (int i = 0; i < trueHistory.Count; i++)
            {
                if (_history.Count == i)
                {
                    Move trueMove = trueHistory[i];
                    _history.Add(trueMove);
                    string throwaway =
                        _engine.SendCommand("play " + (trueMove.WhoMoves == StoneColor.Black ? "B" : "W") + " " +
                                           trueMove.Coordinates.ToIgsCoordinates());
                }
            }
            string movecolor = preMoveInformation.AIColor == StoneColor.Black ? "B" : "W";
            string result = _engine.SendCommand("genmove " + movecolor);
            if (result == "resign")
            {
                return AIDecision.Resign("Resigned because of low win chance.");
            }
            var move = result == "PASS"
                ? Move.Pass(preMoveInformation.AIColor)
                : Move.PlaceStone(preMoveInformation.AIColor, Position.FromIgsCoordinates(result));
            _history.Add(move);
            float value = float.Parse(_engine.SendCommand("uct_value_black"));
            if (preMoveInformation.AIColor == StoneColor.White)
            {
                value = 1 - value;
            }
            return AIDecision.MakeMove(
                move, (Math.Abs(value) < ComparisonTolerance) || (Math.Abs(value - 1) < ComparisonTolerance) ? "Reading from opening book." : "Win chance (" + preMoveInformation.AIColor + "): " + (100*value) + "%");
        }
    }
}