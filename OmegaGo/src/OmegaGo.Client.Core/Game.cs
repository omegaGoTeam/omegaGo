using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    /// <summary>
    /// A game instance represents a game opened in the ingame screen. It might be a game in progress, a watched game or a completed game.
    /// However, a game in Analyze Mode doesn't really need a Game instance, since it's basically just the player operating over a game tree.
    /// On the other hand, information about the game (such as the ruleset) are required in Analyze Mode.... and... well, are stored with SGF files.
    /// 
    /// TODO SGF files might possibly load into Games rather than GameTrees?
    /// 
    /// TODO It is yet to be decided whether a tsumego problem will also qualify as a Game instance. 
    /// </summary>
    class Game
    {
        /// <summary>
        /// In ordinary games, this list will have exactly two players. If we ever add multiplayer games, this could include more players.
        /// </summary>
        public List<Player> Players;
        /// <summary>
        /// The game tree associated with the game. Each game has exactly one associated game tree.
        /// </summary>
        public GameTree GameTree;
    }
}
