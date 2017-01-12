using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class RandomPlayerWrapper : AiProgramBase
    {
        public override string Name { get; } = "Joker23 Random";
        private RandomPlayer internalPlayer;

        public override AiDecision RequestMove(AIPreMoveInformation gameState)
        {
            internalPlayer = new RandomPlayer(gameState.AIColor == StoneColor.Black ? 'B' : 'W');
            char[,] board = JokerExtensionMethods.OurBoardToJokerBoard(gameState.Board, gameState.BoardSize);
            JokerPoint point = internalPlayer.makeMove(board, gameState.BoardSize.Width, gameState.BoardSize.Height);
            return AiDecision.MakeMove(Move.PlaceStone(gameState.AIColor, new Position(point.x, point.y)),
                "I chose at random.");
        }
    }
}