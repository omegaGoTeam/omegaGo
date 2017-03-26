using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class RandomPlayerWrapper : AIProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, false, 1, int.MaxValue);

        private RandomPlayer _internalPlayer;

        public override AIDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            _internalPlayer = new RandomPlayer(preMoveInformation.AIColor == StoneColor.Black ? 'B' : 'W');
            char[,] board = JokerExtensionMethods.OurBoardToJokerBoard(preMoveInformation.GameTree.LastNode.BoardState, preMoveInformation.GameInfo.BoardSize);
            JokerPoint point = _internalPlayer.makeMove(board, preMoveInformation.GameInfo.BoardSize.Width, preMoveInformation.GameInfo.BoardSize.Height);
            return AIDecision.MakeMove(Move.PlaceStone(preMoveInformation.AIColor, new Position(point.x, point.y)),
                "I chose at random.");
        }
    }
}