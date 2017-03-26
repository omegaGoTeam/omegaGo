using System.Linq;
using OmegaGo.Core.AI.Joker23.GameEngine;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class HeuristicPlayerWrapper : AIProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, true, 2, int.MaxValue);        

        public override AIDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            var history = preMoveInformation.GameTree.PrimaryMoveTimeline.ToList();
            if (history.Any() &&
                  history.Last().Kind == MoveKind.Pass)
            {
                return AIDecision.MakeMove(Move.Pass(preMoveInformation.AIColor), "You passed, too!");
            }

            JokerGame currentGame = new JokerGame(preMoveInformation.GameInfo.BoardSize.Height,
                preMoveInformation.GameInfo.BoardSize.Width,
                null,
                null);

            foreach(Move move in history)
            {
                currentGame.moves.AddLast(new JokerMove(move.WhoMoves == StoneColor.Black ? 'B' : 'W',
                    new JokerPoint(move.Coordinates.X, move.Coordinates.Y)));
            }

            currentGame.board = JokerExtensionMethods.OurBoardToJokerBoard(preMoveInformation.GameTree.LastNode.BoardState, preMoveInformation.GameInfo.BoardSize );

            JokerPoint point = new HeuristicPlayer(preMoveInformation.AIColor == StoneColor.Black ? 'B' : 'W').betterPlanMove(currentGame);
            

            return AIDecision.MakeMove(Move.PlaceStone(preMoveInformation.AIColor, new Position(point.x, point.y)),
                "I chose using heuristics.");
        }
    }
}