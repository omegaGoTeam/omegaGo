using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Contains information about a game in progress that AI programs might need.
    /// Data in this class provides the AI with all the infromation it needs to decide on what it thinks is the best move.
    /// </summary>
    public class AiGameInformation
    {
        /// <summary>
        /// Creates a new structure that gives the AI information it needs to make a move.
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="aiColor">The player whose turn it is. The AI will make a move for this player.</param>
        /// <param name="aiPlayer">AI player</param>
        /// <param name="gameTree">The current full board state (excluding information about Ko). </param>
        public AiGameInformation(GameInfo gameInfo, StoneColor aiColor, GamePlayer aiPlayer, GameTree gameTree)
        {
            GameInfo = gameInfo;
            AIColor = aiColor;
            AiPlayer = aiPlayer;
            GameTree = gameTree;
            Node = gameTree.LastNode; // Some concurrency problems may occur here, but they're very unlikely.
            // If we want to solve them, then gameTree.LastNode should be given as an argument to this.
        }

        /// <summary>
        /// Related AI player
        /// </summary>
        public GamePlayer AiPlayer { get; }

        /// <summary>
        /// Game info
        /// </summary>
        public GameInfo GameInfo { get; }
        
        /// <summary>
        /// The player whose turn it is. The AI will make a move for this player.
        /// </summary>
        public StoneColor AIColor { get; }

        /// <summary>
        /// Game tree
        /// </summary>
        public GameTree GameTree { get; }

        /// <summary>
        /// Gets the node to which we're replying.
        /// </summary>
        public GameTreeNode Node { get; }

    }
}
