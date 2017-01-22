using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;

namespace OmegaGo.Core.Modes.LiveGame.Players
{
    /// <summary>
    /// Represents a player participating in a <see cref="Game"/> instance. Does not refer to the user
    /// of the app. A player only ever participates in a single game. The same human playing in multiple games
    /// is represented by different <see cref="GamePlayer"/> instances. 
    /// </summary>
    public abstract class GamePlayer
    {
        /// <summary>
        /// Gets the player type
        /// </summary>
        public GamePlayerType PlayerType { get; }

        /// <summary>
        /// Gets general player metadata
        /// </summary>
        public PlayerInfo Info { get; }
        
        /// <summary>
        /// Agent controlling the player's actions
        /// </summary>
        public abstract IAgent Agent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePlayer"/> class.
        /// </summary>
        /// <param name="playerType">Type of the player</param>
        /// <param name="playerInfo">Information about the player</param>
        protected GamePlayer( GamePlayerType playerType, PlayerInfo playerInfo )
        {
            PlayerType = playerType;
            Info = playerInfo;
        }
    }
}   