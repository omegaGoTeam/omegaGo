using OmegaGo.Core.Game;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsCoordinates
    {
        public static int OurToTheirs(int y, GameBoardSize boardSize)
        {
            return InvertY(y, boardSize);
        }

        public static int TheirsToOurs(int y, GameBoardSize boardSize)
        {
            return InvertY(y, boardSize);
        }
        private static int InvertY(int y, GameBoardSize boardSize)
        {
            return boardSize.Height - y - 1;
        }

    }
}