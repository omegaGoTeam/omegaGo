using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class RandomPlayerWrapper : AIProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, false, 1, int.MaxValue);
        
        public override AIDecision RequestMove(AiGameInformation gameInformation)
        {
            RandomPlayer internalPlayer = new RandomPlayer(gameInformation.AIColor == StoneColor.Black ? 'B' : 'W');
            char[,] board = JokerExtensionMethods.OurBoardToJokerBoard(GetLastNodeOrEmptyBoard(gameInformation.GameTree).BoardState, gameInformation.GameInfo.BoardSize);
            JokerPoint point = internalPlayer.makeMove(board, gameInformation.GameInfo.BoardSize.Width, gameInformation.GameInfo.BoardSize.Height);
            return AIDecision.MakeMove(Move.PlaceStone(gameInformation.AIColor, new Position(point.x, point.y)),
                "I chose at random.");
        }
    }
}