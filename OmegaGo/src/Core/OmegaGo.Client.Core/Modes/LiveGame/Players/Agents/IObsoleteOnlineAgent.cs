﻿using OmegaGo.Core.Game;
using OmegaGo.Core.Online;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    /// <summary>
    /// The online agent is what the server connection classes use to forward decisions from remote players to our client.
    /// </summary>
    interface IObsoleteOnlineAgent
    {
        /// <summary>
        /// This is called by the server connection and informs this agent that the next time it's requested to make a move at
        /// the specified turn number, it should immediately make the move specified by this call instead of how it would usually do it.
        /// This is used to fill out the game history when resuming an online game, for example.
        /// </summary>
        /// <param name="moveIndex">The 1-based turn number.</param>
        /// <param name="move">The move to make at the given turn number.</param>
        void ForceHistoricMove(int moveIndex, Move move);
    }
}
