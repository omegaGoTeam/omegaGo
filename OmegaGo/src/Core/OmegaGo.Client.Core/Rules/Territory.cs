namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Determines which player controlles a specific empty point or region.
    /// </summary>
    public enum Territory
    {
        /// <summary>
        /// This point or region is controlled by the White player.
        /// </summary>
        White,
        /// <summary>
        /// This point or region is controlled by the Black player.
        /// </summary>
        Black,
        /// <summary>
        /// This point or region is bordered by both Black and White stones.
        /// </summary>
        Neutral,
        /// <summary>
        /// The allegiance of this point or region has not yet been calculated.
        /// </summary>
        Unknown
    }
}
