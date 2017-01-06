using OmegaGo.Core.Agents;
using OmegaGo.Core.Game;

namespace OmegaGo.Core
{
    /// <summary>
    /// Represents a player participating in a <see cref="Game"/> instance. Does not refer to the user
    /// of the app. A player only ever participates in a single game. The same human playing in multiple games
    /// is represented by different <see cref="Player"/> instances. 
    /// </summary>
    public class Player
    {


        /// <summary>
        /// The game the player is playing.
        /// </summary>
        private ObsoleteGameInfo Game { get; }
        /// <summary>
        /// Gets the color of the stones this player is using.
        /// </summary>
        public StoneColor Color => Game.Black == this ? StoneColor.Black : StoneColor.White;
        /// <summary>
        /// The number of points this player has scored in the game he participates in.
        /// </summary>
        public int Score;
        /// <summary>
        /// The agent that makes this player's moves.
        /// </summary>
        public IAgent Agent;
        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="name">The player's name.</param>
        /// <param name="rank">The player's rank (e.g. "17k").</param>
        /// <param name="game">The game the player participates in.</param>
        public Player( ObsoleteGameInfo game )
        {
            Game = game;
        }
        /// <summary>
        /// Returns the player's name.
        /// </summary>
        public override string ToString() => Name;
    }
}