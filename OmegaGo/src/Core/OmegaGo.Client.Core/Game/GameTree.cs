using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Represents a game tree
    /// </summary>
    public sealed class GameTree
    {
        public GameTree(IRuleset ruleset)
        {
            Ruleset = ruleset;
        }

        /// <summary>
        /// Ruleset
        /// </summary>
        public IRuleset Ruleset { get; }

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
        /// Gets the primary timeline moves
        /// </summary>
        public IEnumerable<Move> PrimaryMoveTimeline
        {
            get
            {
                return PrimaryTimeline.Select(node => node.Move);
            }
        }

        /// <summary>
        /// Gets the primary timeline
        /// </summary>
        public IEnumerable<GameTreeNode> PrimaryTimeline
        {
            get
            {
                var node = GameTreeRoot;
                while (node != null)
                {
                    yield return node;
                    node = node.NextNode;
                }
            }
        }

        /// <summary>
        /// Adds the move to the end of the primary timeline
        /// </summary>
        /// <param name="move">Move to be added</param>
        /// <param name="boardState">Game board for the move</param>
        public GameTreeNode AddMoveToEnd(Move move, GameBoard boardState)
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
            return node;
        }

        /// <summary>
        /// Adds a new non-move node to the end
        /// </summary>
        /// <param name="newBlackStones">Newly added black stones</param>
        /// <param name="newWhiteStones">Newly added white stones</param>
        /// <param name="gameBoard">Game board</param>
        /// <returns>Newly added node</returns>
        public GameTreeNode AddToEnd(Position[] newBlackStones, Position[] newWhiteStones, GameBoard gameBoard)
        {
            var newNode = AddMoveToEnd(Move.NoneMove, gameBoard);
            newNode.AddBlack.AddRange(newBlackStones);
            newNode.AddWhite.AddRange(newWhiteStones);
            return newNode;
        }

        /// <summary>
        /// Adds a given board to the end of the tree
        /// </summary>
        /// <param name="gameBoard">Game board instance</param>
        /// <returns>Newly added node</returns>
        public GameTreeNode AddBoardToEnd(GameBoard gameBoard)
        {
            return AddMoveToEnd(Move.NoneMove, gameBoard);
        }
    }
}
