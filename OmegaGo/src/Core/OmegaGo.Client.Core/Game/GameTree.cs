using System;
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
        /// <summary>
        /// Contains the last node on the tree's primary timeline
        /// </summary>
        private GameTreeNode _lastNode = null;
        
        /// <summary>
        /// Creates a game tree with a given ruleset
        /// </summary>
        /// <param name="ruleset">Ruleset instance</param>
        public GameTree(IRuleset ruleset)
        {
            Ruleset = ruleset;
        }

        /// <summary>
        /// Indicates that tha last node of the game tree has changed
        /// </summary>
        public event EventHandler<GameTreeNode> LastNodeChanged;

        /// <summary>
        /// Ruleset
        /// </summary>
        public IRuleset Ruleset { get; }

        /// <summary>
        /// Primary timeline length
        /// </summary>
        public int PrimaryTimelineLength { get; private set; } = 0;

        /// <summary>
        /// Game board size
        /// </summary>
        public GameBoardSize BoardSize { get; set; }

        /// <summary>
        /// Root of the game tree
        /// </summary>
        public GameTreeNode GameTreeRoot { get; set; }
        
        /// <summary>
        /// The LastNode is last node of the game's primary timeline and is the node where
        /// the game currently "is". Whenever a player makes a move, the newly created node by that move becomes the LastNode.
        /// Undoing moves will also changes the LastNode. However, working with the Analyze Mode will not change LastNode.
        /// </summary>
        public GameTreeNode LastNode
        {
            get { return _lastNode; }
            set
            {                
                _lastNode = value;
                OnLastNodeChanged();
            }
        }

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
        /// Gets the primary timeline. The primary timeline is the list of nodes from the root up until the LastNode.
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
            return AddMoveToEndInternal(move, boardState);
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

        /// <summary>
        /// Removes the last node from the primary timeline
        /// </summary>
        public void RemoveLastNode()
        {
            //is there actually something to remove?
            if (LastNode == null)
            {
                throw new InvalidOperationException("There is no node to remove from the GameTree.");
            }
            //remove last node, make its parent last
            var previousMove = LastNode.Parent;            
            if (previousMove == null)
            {
                GameTreeRoot = null;
                LastNode = null;
            }
            else
            {
                previousMove.Branches.RemoveNode(LastNode);
                LastNode = previousMove;
            }
        }

        /// <summary>
        /// Implementation of adding a new mode into the primary timeline of the tree
        /// </summary>
        /// <param name="move">Added move</param>
        /// <param name="boardState">State of the board</param>
        /// <returns></returns>
        private GameTreeNode AddMoveToEndInternal(Move move, GameBoard boardState)
        {
            GameTreeNode node = new GameTreeNode(move) { BoardState = boardState };

            if (GameTreeRoot == null)
            {
                GameTreeRoot = node;
            }
            else
            {
                LastNode.Branches.AddNode(node);
                node.Parent = LastNode;
            }
            PrimaryTimelineLength++;
            LastNode = node;
            return node;
        }

        /// <summary>
        /// Fires the <see cref="LastNodeChanged" /> event
        /// </summary>
        private void OnLastNodeChanged()
        {
            LastNodeChanged?.Invoke(this, LastNode);
        }
    }
}
