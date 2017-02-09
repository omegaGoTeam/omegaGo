﻿using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Time;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Local;
using OmegaGo.Core.Modes.LiveGame.State;

namespace OmegaGo.Core.Modes.LiveGame.Players
{
    /// <summary>
    /// Represents a player participating in a <see cref="Game"/> instance.
    /// The same human playing in multiple games
    /// is represented by different <see cref="GamePlayer"/> instances. 
    /// </summary>
    public class GamePlayer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GamePlayer"/> class.
        /// </summary>
        /// <param name="playerInfo">Information about the player</param>
        /// <param name="agent">Agent</param>
        public GamePlayer( PlayerInfo playerInfo, IAgent agent, TimeControl clock)
        {
            Info = playerInfo;
            Agent = agent;
            Clock = clock;
        }

        /// <summary>
        /// Gets general player metadata
        /// </summary>
        public PlayerInfo Info { get; }

        /// <summary>
        /// Agent controlling the player's actions
        /// </summary>
        public IAgent Agent { get; }

        /// <summary>
        /// Player's clock
        /// </summary>
        public TimeControl Clock { get; }

        /// <summary>
        /// Assigns the player to a game
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="gameState">Game state</param>
        public void AssignToGame(GameInfo gameInfo, IGameState gameState)
        {
            Agent.AssignToGame(gameInfo, gameState);
        }

        /// <summary>
        /// Returns player's name
        /// </summary>
        /// <returns>Player's name</returns>
        public override string ToString() => Info.Name;
    }
}   