using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.Common;

namespace OmegaGo.Core.AI.Random
{
    public class RandomAI : AiProgramBase
    {
        

        private System.Random rgen = new System.Random();
        public override string Name => "Random";

        public override Task<AIDecision> RequestMove(AIPreMoveInformation gameState)
        {
            List<Position> possibleIntersections = FastBoard.GetAllLegalMoves(gameState.Board);
            if (possibleIntersections.Count == 0)
            {
                return Task.FromResult(AIDecision.Resign("There are no more moves left to do."));
            }
            Position chosen = possibleIntersections[rgen.Next(possibleIntersections.Count)];
            return Task.FromResult(AIDecision.MakeMove(new Core.Move()
            {
                WhoMoves = gameState.AIColor,
                Coordinates = chosen
            }, "I chose at random."));
        }
    }
}
