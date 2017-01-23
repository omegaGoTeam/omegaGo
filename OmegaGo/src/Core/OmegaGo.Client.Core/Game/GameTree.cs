using System.Collections.Generic;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Represents a game tree
    /// </summary>
    public sealed class GameTree
    {
        /// <summary>
        /// Game board size
        /// </summary>
        public GameBoardSize BoardSize { get; set; }

        /// <summary>
        /// Root of the game tree
        /// </summary>
        public GameTreeNode GameTreeRoot { get; set; }

        /// <summary>
        /// Reference to the last added node
        /// </summary>
        public GameTreeNode LastNode { get; set; }

        /// <summary>
        /// Gets the primary timeline
        /// </summary>
        public IEnumerable<Move> PrimaryMoveTimeline
        {
            get
            {
                GameTreeNode node = GameTreeRoot;
                while (node != null)
                {
                    yield return node.Move;
                    node = node.NextMove;
                }
            }
        }

        /// <summary>
        /// Adds the move to the end of the tree
        /// </summary>
        /// <param name="move">Move to be added</param>
        /// <param name="boardState">Game board for the move</param>
        public void AddMoveToEnd(Move move, GameBoard boardState)
        {
            GameTreeNode node = new GameTreeNode(move);
            node.BoardState = boardState;
            
            if (GameTreeRoot == null)
            {
                GameTreeRoot = node;
            }
            else
            {
                LastNode.Branches.AddNode(node);
                node.Parent = LastNode;
            }

            LastNode = node;
        }
    }
}
