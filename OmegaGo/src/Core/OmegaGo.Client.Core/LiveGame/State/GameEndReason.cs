namespace OmegaGo.Core.Modes.LiveGame.State
{
    /// <summary>
    /// Describes the reason why the game has ended
    /// </summary>
    public enum GameEndReason
    {
        /// <summary>
        /// The game ended through normal progress of gameplay.
        /// </summary>
        ScoringComplete,
        /// <summary>
        /// A player lost on time because they did not have any time left.
        /// </summary>
        Timeout,
        /// <summary>
        /// A player has resigned.
        /// </summary>
        Resignation,
        /// <summary>
        /// A player has disconnected from the server. Alternatively, if we are spectating a game, this may also mean
        /// that while the game is still in progress, we have lost connection to the server.
        /// </summary>
        Disconnection,
        /// <summary>
        /// The game was cancelled without a result, perhaps because the server
        /// decided that it will be so or because it was a local game that was aborted
        /// by user without result, or perhaps because the user just exited the game.
        /// </summary>
        Cancellation
    }
}
