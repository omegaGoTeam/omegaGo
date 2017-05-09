using System.Linq;
using OmegaGo.Core.AI.Joker23.GameEngine;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class HeuristicPlayerWrapper : AIProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, true, 9, int.MaxValue);        

        public override AIDecision RequestMove(AiGameInformation gameInformation)
        {
            var history = gameInformation.GameTree.PrimaryMoveTimeline.ToList();
            if (history.Any() &&
                  history.Last().Kind == MoveKind.Pass)
            {
                return AIDecision.MakeMove(Move.Pass(gameInformation.AIColor), "You passed, too!");
            }

            JokerGame currentGame = new JokerGame(gameInformation.GameInfo.BoardSize.Height,
                gameInformation.GameInfo.BoardSize.Width,
                null,
                null);

            foreach(Move move in history)
            {
                currentGame.moves.AddLast(new JokerMove(move.WhoMoves == StoneColor.Black ? 'B' : 'W',
                    new JokerPoint(move.Coordinates.X, move.Coordinates.Y)));
            }

            currentGame.board = JokerExtensionMethods.OurBoardToJokerBoard(GetLastNodeOrEmptyBoard(gameInformation.GameTree).BoardState, gameInformation.GameInfo.BoardSize );

            JokerPoint point = new HeuristicPlayer(gameInformation.AIColor == StoneColor.Black ? 'B' : 'W').betterPlanMove(currentGame);
            

            return AIDecision.MakeMove(Move.PlaceStone(gameInformation.AIColor, new Position(point.x, point.y)),
                "I chose using heuristics.");
        }
    }
}