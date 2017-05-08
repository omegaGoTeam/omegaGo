namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    /// <summary>
    /// Specifies the type of the agent
    /// </summary>
    public enum AgentType
    {
        /// <summary>
        /// The agent is a human using this device.
        /// </summary>
        Human,
        /// <summary>
        /// The agent is a local AI program that's part of omegaGo.
        /// </summary> 
        AI,
        /// <summary>
        /// The agent is an online player with a different username.
        /// </summary>
        Remote
    }
}
