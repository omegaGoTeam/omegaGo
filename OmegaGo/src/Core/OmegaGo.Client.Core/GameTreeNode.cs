using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    /// <summary>
    /// Represents a node in a game tree. In games, the game tree will be a "path" of <see cref="GameTreeNode"/>s
    /// with each node having only a single child. In game analysis, the game tree may be an actual tree.
    /// </summary>
    public sealed class GameTreeNode
    {
        // Information taken from official SGF file definition
        // http://www.red-bean.com/sgf/proplist_ff.html
        // and SGF file examples
        // http://www.red-bean.com/sgf/examples/

        public string Comment { get; set; }
        public string Name { get; set; }

        public List<string> AddBlack { get; set; }
        public List<string> AddWhite { get; set; }

        /// <summary>
        /// Describes current state of the entire game board. Can be null.
        /// </summary>
        public GameBoard BoardState { get; set; }

        // Contain territory
        // public List<Shape> Figures { get; set; } - Implement Shape 
        public List<KeyValuePair<Position, string>> Labels { get; set; }

        /// <summary>
        /// Gets or sets the move that caused this <see cref="GameTreeNode"/> to exist. 
        /// </summary>
        public Move Move { get; set; }

        /// <summary>
        /// Gets or sets the parent node of this node, i.e. the move before this one.
        /// </summary>
        public GameTreeNode Parent { get; set; }

        /*
         *  When there is more than one recorded move after a move, 
         *  always branch:
         *  (;W[dd]N[W d16]     // White plays
         *  (;B[pp]N[B q4])     // Black plays - Possible Move A
         *  (;B[dp]N[B d4]))    // Black plays - Possible Move B
        */
        /// <summary>
        /// Gets or sets the children of this node, i.e. the nodes/moves that follow this move.
        /// In normal games, there will only be a single element in this list.
        /// </summary>
        public GameTreeNodeCollection Branches { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this node has no children, which usually means that it's the last move made.
        /// </summary>
        public bool IsLeafNode => Branches.Count == 0;

        /// <summary>
        /// Gets the move number of this node by moving up the chain of nodes until the root - takes O(N) time. 
        /// The move number is 1-based, i.e. the first move has the number "1".
        /// </summary>
        public int MoveNumber
        {
            get
            {
                int number = 0;
                GameTreeNode node = this;
                while (node != null)
                {
                    node = node.Parent;
                    number++;
                }
                return number;
            }
        }
        /// <summary>
        /// Gets the only child node of this node, if it exists, otherwise null. Throws if there are two or more children.
        /// </summary>
        /// <exception cref="InvalidOperationException">When this is a branching node.</exception>
        public GameTreeNode NextMove
        {
            get
            {
                switch (Branches.Count)
                {
                    case 0: return null;
                    case 1: return Branches[0];
                    default: throw new InvalidOperationException("This is a branching node. Therefore, there is no single 'next' move.");
                }
            }
        }

        public GameTreeNode(Move move)
        {
            this.Branches = new GameTreeNodeCollection(this);
            this.Move = move;
        }

        public IEnumerable<GameTreeNode> GetTimelineView
        {
            get
            {
                yield return this;
                GameTreeNode node = this.NextMove;
                while (node != null)
                {
                    yield return node;
                    node = node.NextMove;
                }
            }
        }

        /// <summary>
        /// Gets the list of all moves that lead to the provided node.
        /// The list is starting with root node.
        /// </summary>
        /// <param name="node">target node</param>
        /// <param name="filterNonMoves">determines whether nodes with MoveKind.None should be included</param>
        /// <returns>nodes history</returns>
        public static List<GameTreeNode> GetNodeHistory(GameTreeNode node, bool filterNonMoves)
        {
            if (node == null)
                throw new ArgumentNullException("Node cant be null");

            List<GameTreeNode> nodeHistory = new List<GameTreeNode>();

            do
            {
                if (filterNonMoves && (node.Move.Kind == MoveKind.Pass || node.Move.Kind == MoveKind.PlaceStone))
                    nodeHistory.Insert(0, node);
                else
                    nodeHistory.Insert(0, node);

                node = node.Parent;
            } while (node != null);

            return nodeHistory;
        }
    }
}
