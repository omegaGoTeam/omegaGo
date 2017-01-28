namespace OmegaGo.Core.Game
{
    /// <summary>
    /// There are two kinds of moves - placing a stone; and passing.
    /// </summary>
    public enum MoveKind
    {
        /// <summary>
        /// Used for SGF, where not every node has a move defined.
        /// </summary>
        None,
        /// <summary>
        /// The most common move - a player places a stone on the board.
        /// </summary>
        PlaceStone,
        /// <summary>
        /// The other move - a player passes to signal that they think the game is over.
        /// </summary>
        Pass
    }
}
