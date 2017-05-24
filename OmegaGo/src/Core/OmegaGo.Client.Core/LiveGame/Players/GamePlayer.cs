using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.LiveGame.Players.Agents;
using OmegaGo.Core.Time;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
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
        /// <param name="clock">Time control system for this player, including time at the beginning.</param>
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
        // TODO (future work) Petr (low importance): If time is available, and KGS does not use the TIMESYSTEM property during a game, consider refactoring this so that the setter is not necessary.
        public TimeControl Clock { get; internal set; }

        /// <summary>
        /// Checks if the player is a local human player
        /// </summary>
        public bool IsHuman => Agent.Type == AgentType.Human;

        /// <summary>
        /// Checks if the player is a local player (either AI or Human)
        /// </summary>
        public bool IsLocal => Agent.Type != AgentType.Remote;     

        /// <summary>
        /// Assigns the player to a game
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="gameState">Game state</param>
        public void AssignToGame(GameInfo gameInfo, IGameState gameState)
        {
            //assign the agent player to this game
            Agent.AssignToGame(gameInfo, gameState);
        }

        /// <summary>
        /// Returns player's name
        /// </summary>
        /// <returns>Player's name</returns>
        public override string ToString() => Info.ToString();
    }
}   