using OmegaGo.Core.Agents;
using OmegaGo.Core.Game;

namespace OmegaGo.Core
{
    /// <summary>
    /// Represents a player participating in a <see cref="Game"/> instance. Does not refer to the user
    /// of the app. A player only ever participates in a single game. The same human playing in multiple games
    /// is represented by different <see cref="GamePlayer"/> instances. 
    /// </summary>
    public class GamePlayer
    {
        public GamePlayerInfo Info { get; }
        
        /// <summary>
        /// The agent that makes this player's moves.
        /// </summary>
        public IAgent Agent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GamePlayer"/> class.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="rank">The player's rank (e.g. "17k").</param>
        /// <param name="game">The game the player participates in.</param>
        public GamePlayer(GamePlayerInfo playerInfo, IAgent agent )
        {
            Info = playerInfo;
            Agent = agent;
        }
    }
}