using OmegaGo.Core.Agents;

namespace OmegaGo.Core
{
    /// <summary>
    /// Represents a player participating in a <see cref="Game"/> instance. Does not refer to the user
    /// of the app. A player only ever participates in a single game. The same human playing in multiple games
    /// is represented by different <see cref="Player"/> instances. 
    /// </summary>
    public class Player
    {
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the rank of the player. There should be no whitespace. The rank may be arbitrary otherwise: NR, 17k, 6d+, 5p? etc.
        /// </summary>
        public string Rank { get; set; }
        /// <summary>
        /// The number of points this player has scored in the game he participates in.
        /// </summary>
        public int Score;
        /// <summary>
        /// The agent that makes this player's moves.
        /// </summary>
        public IAgent Agent;

        public Player(string name, string rank)
        {
            Name = name;
            Rank = rank;
        }
        public override string ToString() => Name;
    }
}