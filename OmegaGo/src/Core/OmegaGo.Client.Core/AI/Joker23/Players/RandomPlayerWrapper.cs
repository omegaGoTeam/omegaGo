using OmegaGo.Core.Game;

namespace OmegaGo.Core.AI.Joker23.Players
{
    public class RandomPlayerWrapper : AiProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, true, 1, int.MaxValue);
        public override string Name { get; } = "The Fish";
        public override string Description => "This AI plays random moves while the board is not empty.\n\nI dare you to let two Fish play against each other. The game will never end (unless you're very lucky).";

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