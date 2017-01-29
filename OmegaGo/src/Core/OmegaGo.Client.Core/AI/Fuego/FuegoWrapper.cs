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
    /// <seealso cref="OmegaGo.Core.AI.AiProgramBase" />
    class FuegoWrapper : AiProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, false, 2, 19);
        public override string Name => "Fuego (recommended)";

        public override string Description
            =>
                "Fuego is a well-known open-source Go-playing engine written at the University of Alberta in Canada.\n\nIt uses Monte Carlo tree search to make moves. It's capable of placing stones, passing and resigning, as the situation calls for.\n\nWe recommend you use this AI program for all of your games."
            ;

        private IGtpEngine engine;
        private bool initialized;
        private int timelimit = -1;
        private List<Move> history = new List<Move>();

        public override AiDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            if (!initialized)
            {
                engine = AISystems.FuegoBuilder.CreateEngine(preMoveInformation.Board.Size.Width);
                engine.SendCommand("uct_param_player ponder 1");
                // TODO komi
                initialized = true;
            }
            if (preMoveInformation.Difficulty != timelimit)
            {
                engine.SendCommand("go_param timelimit " + preMoveInformation.Difficulty);
                timelimit = preMoveInformation.Difficulty;
            }
            var trueHistory = preMoveInformation.History.ToList();
            for (int i = 0; i < trueHistory.Count; i++)
            {
                if (history.Count == i)
                {
                    Move trueMove = trueHistory[i];
                    history.Add(trueMove);
                    string throwaway =
                        engine.SendCommand("play " + (trueMove.WhoMoves == StoneColor.Black ? "B" : "W") + " " +
                                           trueMove.Coordinates.ToIgsCoordinates());
                }
            }
            string movecolor = preMoveInformation.AIColor == StoneColor.Black ? "B" : "W";
            string result = engine.SendCommand("genmove " + movecolor);
            if (result == "resign")
            {
                return AiDecision.Resign("Resigned because of low win chance.");
            }
            var move = result == "PASS"
                ? Move.Pass(preMoveInformation.AIColor)
                : Move.PlaceStone(preMoveInformation.AIColor, Position.FromIgsCoordinates(result));
            history.Add(move);
            float value = float.Parse(engine.SendCommand("uct_value_black"));
            if (preMoveInformation.AIColor == StoneColor.White)
            {
                value = 1 - value;
            }
            return AiDecision.MakeMove(
                move, (value == 0) || (value == 1) ? "Reading from opening book." : "Win chance (" + preMoveInformation.AIColor + "): " + (100*value) + "%");
        }
    }
}
