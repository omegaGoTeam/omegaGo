namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Represents a color of stones in a game of two-player Go. This enumeration is used both for identifying players
    /// and the stones placed on the board.
    /// </summary>
    public enum StoneColor : byte
    {
        /// <summary>
        /// An intersection has the color <see cref="None"/> if there is no stone placed upon it. 
        /// It is NECESSARY for this enum value to be the first so that boards are initialized with it.
        /// </summary>
        None = 0,
        /// <summary>
        /// The player who plays black stones always goes first. In games outside tournaments, Black is usually the
        /// weaker player of the two and receives handicap stones or compensation points to make the match more even.
        /// </summary>
        Black,
        /// <summary>
        /// The player who plays white stones always goes second. In some ruleses (specifically, AGA), the white player
        /// is required to make the last pass.
        /// </summary>
        White
    }
}
