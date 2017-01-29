﻿namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    /// <summary>
    /// Determines what the game controller should do in case the agent submits a move that's not legal under the ruleset.
    /// </summary>
    public enum IllegalMoveHandling
    {
        /// <summary>
        /// The agent should be prompted to make a move one more time. This is how user interface agents should act.
        /// </summary>
        InformAgent,
        /// <summary>
        /// A random move should be made instead since a retry would likely produce the same result. This is how AI's should act.
        /// </summary>
        MakeRandomMove,
        /// <summary>
        /// The agent is smarter than the game controller and its moves should always be accepted as legal.
        /// </summary>
        PermitItAnyway
    }
}