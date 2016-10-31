namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Represents the result of a legality check that determines whether a single move is in accordance with the rules.
    /// </summary>
    public enum MoveResult
    {
        /// <summary>
        /// The move is legal and can be made.
        /// </summary>
        Legal,
        /// <summary>
        /// The move is illegal because it attempted to place a stone on an intersection 
        /// that is already occupied by another stone.
        /// </summary>
        OccupiedPosition,
        /// <summary>
        /// The move is illegal because of the Ko rule - it places a stone on the intersection where the opponent
        /// has just captured a stone and causes the same full board position to appear again.
        /// </summary>
        Ko,
        /// <summary>
        /// The move is illegal because of a superko rule - it causes a full board position to repeat again in the
        /// same game. If a move is illegal because of a simple Ko, then <see cref="Ko"/> will be returned and not
        /// <see cref="SuperKo"/>.  
        /// </summary>
        SuperKo,
        /// <summary>
        /// The move is illegal because it is suicidal - it would not cause any capture of opponent's stones and, 
        /// conversely, the stone it placed would be immediately captured.
        /// </summary>
        SelfCapture,
        /// <summary>
        /// Two consecutive passes signal the end of the game. After two passes, the players marks stones as life or dead in the Confirmation phase.
        /// </summary>
        LifeDeadConfirmationPhase
    }
}