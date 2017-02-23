namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Represents the kind of the decision: whether it's making a move or resigning.
    /// </summary>
    public enum AgentDecisionKind
    {
        /// <summary>
        /// The agent wishes to make a move - either to place a stone or to pass.
        /// </summary>
        Move,
        /// <summary>
        /// Only AI programs will use the "resign" option. Human players will use a different channel to resign.
        /// </summary>
        Resign
    }
}