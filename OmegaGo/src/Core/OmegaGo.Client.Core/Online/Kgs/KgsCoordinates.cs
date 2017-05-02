using OmegaGo.Core.Game;

namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// This helper class inverts Y-coordinates because KGS counts coordiantes from the topleft corner rather than from A1.
    /// </summary>
    public static class KgsCoordinates
    {
        /// <summary>
        /// Converts an OmegaGo Y coordinate to an KGS Y coordinate.
        /// </summary>
        /// <returns></returns>
        public static int OurToTheirs(int y, GameBoardSize boardSize)
        {
            return InvertY(y, boardSize);
        }

        /// <summary>
        /// Converts a KGS Y coordinate to an OmegaGo Y coordinate.
        /// </summary>
        /// <returns></returns>
        public static int TheirsToOurs(int y, GameBoardSize boardSize)
        {
            return InvertY(y, boardSize);
        }
        /// <summary>
        /// Yes, the methods do the same thing.
        /// </summary>
        private static int InvertY(int y, GameBoardSize boardSize)
        {
            return boardSize.Height - y - 1;
        }

    }
}