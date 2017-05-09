using System.Linq;
using OmegaGo.Core.AI.Joker23.GameEngine;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class Fluffy : AIProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, true, 9, int.MaxValue);

        public int TreeDepth { get; set; }

        public override AIDecision RequestMove(AiGameInformation gameInformation)
        {
            var moves = gameInformation.GameTree.PrimaryMoveTimeline.ToList();

            if (moves.Any() &&
               moves.Last().Kind == MoveKind.Pass)
            {
                return AIDecision.MakeMove(Move.Pass(gameInformation.AIColor), "You passed, too!");
            }

            JokerGame currentGame = new JokerGame(gameInformation.GameInfo.BoardSize.Height,
                gameInformation.GameInfo.BoardSize.Width,
                null,
                null);

            foreach(Move move in gameInformation.GameTree.PrimaryMoveTimeline)
            {
                currentGame.moves.AddLast(new JokerMove(move.WhoMoves == StoneColor.Black ? 'B' : 'W',
                    new JokerPoint(move.Coordinates.X, move.Coordinates.Y)));
            }

            currentGame.board = JokerExtensionMethods.OurBoardToJokerBoard(GetLastNodeOrEmptyBoard(gameInformation.GameTree).BoardState, gameInformation.GameInfo.BoardSize);

            JokerPoint point = new AlphaBetaPlayer(gameInformation.AIColor == StoneColor.Black ? 'B' : 'W').betterPlanMove(currentGame, this.TreeDepth);
            

            return AIDecision.MakeMove(Move.PlaceStone(gameInformation.AIColor, new Position(point.x, point.y)),
                "I chose using the minimax algorithm and heuristics.");
        }
    }
}