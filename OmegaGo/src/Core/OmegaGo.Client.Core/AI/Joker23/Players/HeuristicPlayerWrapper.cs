using OmegaGo.Core.AI.Joker23.GameEngine;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class HeuristicPlayerWrapper : AiProgramBase
    {
        public override string Name { get; } = "Joker23 Heuristic";
        private HeuristicPlayer internalPlayer;

        public override AiDecision RequestMove(AIPreMoveInformation gameState)
        {
            internalPlayer = new HeuristicPlayer(gameState.AIColor == StoneColor.Black ? 'B' : 'W');

            JokerGame currentGame = new JokerGame(gameState.BoardSize.Height,
                gameState.BoardSize.Width,
                null,
                null);

            foreach(Move move in gameState.History)
            {
                currentGame.moves.AddLast(new JokerMove(move.WhoMoves == StoneColor.Black ? 'B' : 'W',
                    new JokerPoint(move.Coordinates.X, move.Coordinates.Y)));
            }

            currentGame.board = JokerExtensionMethods.OurBoardToJokerBoard(gameState.Board, gameState.BoardSize);

            JokerPoint point = internalPlayer.betterPlanMove(currentGame);
            

            return AiDecision.MakeMove(Move.PlaceStone(gameState.AIColor, new Position(point.x, point.y)),
                "I chose using heuristics.");
        }
    }
}