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

        public override AIDecision RequestMove(AiGameInformation gameInformation)
        {
            var moves = gameInformation.GameTree.PrimaryMoveTimeline.ToList();
            if (moves.Any() &&
               moves.Last().Kind == MoveKind.Pass)
            {
                return AIDecision.MakeMove(Move.Pass(gameInformation.AIColor), "You passed, too!");
            }
            GameBoard createdBoard = GameBoard.CreateBoardFromGameTree(gameInformation.GameInfo, gameInformation.GameTree);
            List<Position> possibleIntersections = 
                Ruleset.Create(
                    gameInformation.GameInfo.RulesetType, 
                    gameInformation.GameInfo.BoardSize).GetAllLegalMoves(gameInformation.AIColor, createdBoard, gameInformation.GameTree.LastNode.GetGameBoardHistory().ToArray());
            if (possibleIntersections.Count == 0)
            {
                return AIDecision.Resign("There are no more moves left to do.");
            }
            Position chosen = possibleIntersections[Randomizer.Next(possibleIntersections.Count)];
            return AIDecision.MakeMove(Move.PlaceStone(gameInformation.AIColor, chosen), "I chose at random.");
        }
    }
}
