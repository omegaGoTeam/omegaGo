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
    /// <summary>
    /// This example basic AI will make random legal moves.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.AI.AIProgramBase" />
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
            List<Position> possibleIntersections = 
                Ruleset.Create(
                    preMoveInformation.GameInfo.RulesetType, 
                    preMoveInformation.GameInfo.BoardSize).GetAllLegalMoves(preMoveInformation.AIColor, createdBoard, preMoveInformation.GameTree.LastNode.GetGameBoardHistory().ToArray());
            if (possibleIntersections.Count == 0)
            {
                return AIDecision.Resign("There are no more moves left to do.");
            }
            Position chosen = possibleIntersections[Randomizer.Next(possibleIntersections.Count)];
            return AIDecision.MakeMove(Move.PlaceStone(preMoveInformation.AIColor, chosen), "I chose at random.");
        }
    }
}
