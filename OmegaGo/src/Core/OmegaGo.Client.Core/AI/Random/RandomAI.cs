using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Common;
using OmegaGo.Core.Game;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.AI.Random
{
    public class RandomAI : AiProgramBase
    {
        

        private System.Random rgen = new System.Random();
        public override string Name => "Random";

        public override AiDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            //TODO: The all legal moves request is not correct
            List<Position> possibleIntersections = new ChineseRuleset(preMoveInformation.Board.Size).GetAllLegalMoves(preMoveInformation.Board);
            if (possibleIntersections.Count == 0)
            {
                return AiDecision.Resign("There are no more moves left to do.");
            }
            Position chosen = possibleIntersections[rgen.Next(possibleIntersections.Count)];
            return AiDecision.MakeMove(Move.PlaceStone(preMoveInformation.AIColor, chosen), "I chose at random.");
        }
    }
}
