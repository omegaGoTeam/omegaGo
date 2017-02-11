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
    internal class FuegoWrapper : AIProgramBase
    {
        private readonly List<Move> _history = new List<Move>();

        private bool _initialized;
        private IGtpEngine _engine;
        private int _timelimit = -1;
        
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
                move, (value == 0) || (value == 1) ? "Reading from opening book." : "Win chance (" + preMoveInformation.AIColor + "): " + (100*value) + "%");
        }

        /// <summary>
        /// Name of the AI
        /// </summary>
        public override string Name => "Fuego (recommended)";

        /// <summary>
        /// Capabilities of the AI
        /// </summary>
        public override AICapabilities Capabilities => new AICapabilities(false, false, 2, 19);

        /// <summary>
        /// Description of the AI
        /// </summary>
        public override string Description
            => @"Fuego is a well-known open-source Go-playing engine written at the University of Alberta in Canada.\n\n
                 It uses Monte Carlo tree search to make moves. It's capable of placing stones, passing and resigning, as the situation calls for.\n\n
                 We recommend you use this AI program for all of your games.";
    }
}
