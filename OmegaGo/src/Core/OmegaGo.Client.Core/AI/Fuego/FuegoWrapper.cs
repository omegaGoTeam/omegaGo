using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Fuego
{
    class FuegoWrapper : AiProgramBase
    {
        public override string Name => "Fuego";

        private IGtpEngine engine;
        private bool initialized;

        private List<Move> history = new List<Move>();

        public override AiDecision RequestMove(AIPreMoveInformation gameState)
        {
            if (!initialized)
            {
                engine = AISystems.FuegoBuilder.CreateEngine(gameState.BoardSize.Width);
                initialized = true;
            }
            var trueHistory = gameState.History.ToList();
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
            string movecolor = gameState.AIColor == StoneColor.Black ? "B" : "W";
            string result = engine.SendCommand("genmove " + movecolor);
            return AiDecision.MakeMove(
                result == "PASS" ? Move.Pass(gameState.AIColor) :
                Move.PlaceStone(gameState.AIColor, Position.FromIgsCoordinates(result)), "Fuego");
        }
    }
}
