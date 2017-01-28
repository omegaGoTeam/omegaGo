using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class RandomPlayerWrapper : AiProgramBase
    {
        public override string Name { get; } = "Joker23 Random";

        private RandomPlayer _internalPlayer;

        public override AiDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            _internalPlayer = new RandomPlayer(preMoveInformation.AIColor == StoneColor.Black ? 'B' : 'W');
            char[,] board = JokerExtensionMethods.OurBoardToJokerBoard(preMoveInformation.Board, preMoveInformation.Board.Size);
            JokerPoint point = _internalPlayer.makeMove(board, preMoveInformation.Board.Size.Width, preMoveInformation.Board.Size.Height);
            return AiDecision.MakeMove(Move.PlaceStone(preMoveInformation.AIColor, new Position(point.x, point.y)),
                "I chose at random.");
        }
    }
}