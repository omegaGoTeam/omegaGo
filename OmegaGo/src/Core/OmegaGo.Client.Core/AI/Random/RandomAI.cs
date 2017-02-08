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
    public class RandomAI : AiProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(true, true, 1, int.MaxValue);
        public override string Name => "Random";

        public override string Description
            =>
                "This AI will select a random position at which it can play and it will play there. It will pass as soon its opponent passes."
            ;

        public override AiDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            var moves = preMoveInformation.GameTree.PrimaryMoveTimeline.ToList();
            if (moves.Any() &&
               moves.Last().Kind == MoveKind.Pass)
            {
                return AiDecision.MakeMove(Move.Pass(preMoveInformation.AIColor), "You passed, too!");
            }
            GameBoard createdBoard = GameBoard.CreateBoardFromGameTree(preMoveInformation.GameInfo, preMoveInformation.GameTree);
            List<Position> possibleIntersections = 
                Ruleset.Create(
                    preMoveInformation.GameInfo.RulesetType, 
                    preMoveInformation.GameInfo.BoardSize).GetAllLegalMoves(preMoveInformation.AIColor, createdBoard, preMoveInformation.GameTree.LastNode.GetNodeHistory().ToArray());
            if (possibleIntersections.Count == 0)
            {
                return AiDecision.Resign("There are no more moves left to do.");
            }
            Position chosen = possibleIntersections[Randomizer.Next(possibleIntersections.Count)];
            return AiDecision.MakeMove(Move.PlaceStone(preMoveInformation.AIColor, chosen), "I chose at random.");
        }
    }
}
