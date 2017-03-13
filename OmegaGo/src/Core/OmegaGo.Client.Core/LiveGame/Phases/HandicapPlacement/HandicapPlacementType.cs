namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement
{
    /// <summary>
    /// Describes the type of handicap placement used
    /// </summary>
    public enum HandicapPlacementType
    {
        /// <summary>
        /// Handicap is placed freely based on the player's decision
        /// </summary>
        Free,
        /// <summary>
        /// Handicap positions are fixed (only supported for 9, 13 and 19 boards)
        /// </summary>
        Fixed
    }
}
