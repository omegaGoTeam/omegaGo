using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Helpers;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.AI.Random
{
    public class RandomAI : AIProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(true, true, 1, int.MaxValue);

        public override AIDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            var moves = preMoveInformation.GameTree.PrimaryMoveTimeline.ToList();
            if (moves.Any() &&
               moves.Last().Kind == MoveKind.Pass)
            {
                return AIDecision.MakeMove(Move.Pass(preMoveInformation.AIColor), "You passed, too!");
            }
            GameBoard createdBoard = GameBoard.CreateBoardFromGameTree(preMoveInformation.GameInfo, preMoveInformation.GameTree);
            MoveResult[,] moveResults = 
                Ruleset.Create(
                    preMoveInformation.GameInfo.RulesetType, 
                    preMoveInformation.GameInfo.BoardSize,CountingType.Area).GetMoveResult(preMoveInformation.GameTree.LastNode);
            List<Position> possibleIntersections = new List<Position>();
            for (int x = 0; x < preMoveInformation.GameInfo.BoardSize.Width; x++)
                for (int y = 0; y < preMoveInformation.GameInfo.BoardSize.Height; y++)
                    if (moveResults[x, y] == MoveResult.Legal)
                        possibleIntersections.Add(new Position(x, y));

            if (possibleIntersections.Count == 0)
            {
                return AIDecision.Resign("There are no more moves left to do.");
            }
            Position chosen = possibleIntersections[Randomizer.Next(possibleIntersections.Count)];
            return AIDecision.MakeMove(Move.PlaceStone(preMoveInformation.AIColor, chosen), "I chose at random.");
            //TODO Aniko: ask Petr, whether we need to check the legality(because of superko)
        }
    }
}
