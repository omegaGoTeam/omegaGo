using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;

namespace OmegaGo.Core.Modes.LiveGame.Players
{
    /// <summary>
    /// Represents a player participating in a <see cref="Game"/> instance. Does not refer to the user
    /// of the app. A player only ever participates in a single game. The same human playing in multiple games
    /// is represented by different <see cref="GamePlayer"/> instances. 
    /// </summary>
    public class GamePlayer
    {
        public PlayerInfo Info { get; }
        
        /// <summary>
        /// The agent that makes this player's moves.
        /// </summary>
        public IObsoleteAgent Agent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePlayer"/> class.
        /// </summary>
        /// <param name="playerInfo">Information about the player</param>
        /// <param name="agent">Agent controlling the player</param>
        public GamePlayer(PlayerInfo playerInfo, IObsoleteAgent agent )
        {
            Info = playerInfo;
            Agent = agent;
        }
    }
}