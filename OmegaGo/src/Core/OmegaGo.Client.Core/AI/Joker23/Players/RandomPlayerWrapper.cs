using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Joker23
{
    public class RandomPlayerWrapper : AiProgramBase
    {
        public override string Name { get; } = "Joker23 Random";
        private RandomPlayer internalPlayer;

        public override AgentDecision RequestMove(AIPreMoveInformation gameState)
        {
            internalPlayer = new Joker23.RandomPlayer(gameState.AIColor == Color.Black ? 'B' : 'W');
            char[,] board = JokerExtensionMethods.OurBoardToJokerBoard(gameState.Board, gameState.BoardSize);
            JokerPoint point = internalPlayer.makeMove(board, gameState.BoardSize.Width, gameState.BoardSize.Height);
            return AgentDecision.MakeMove(Move.Create(gameState.AIColor, new Position(point.x, point.y)),
                "I chose at random.");
        }
    }
}