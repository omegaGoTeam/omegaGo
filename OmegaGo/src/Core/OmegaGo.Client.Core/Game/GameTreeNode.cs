﻿using System;
using System.Collections.Generic;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Represents a node in a game tree. In games, the game tree will be a "path" of <see cref="GameTreeNode"/>s
    /// with each node having only a single child. In game analysis, the game tree may be an actual tree.
    /// </summary>
    public sealed class GameTreeNode
    {
        private readonly IRuleset _ruleset;

        public GameTreeNode( IRuleset ruleset )
        {
            _ruleset = ruleset;
        }

        // Information taken from official SGF file definition
        // http://www.red-bean.com/sgf/proplist_ff.html
        // and SGF file examples
        // http://www.red-bean.com/sgf/examples/

        public string Comment { get; set; }
        public string Name { get; set; }

        public List<Position> AddBlack { get; set; } = new List<Position>();
        public List<Position> AddWhite { get; set; } = new List<Position>();

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
        public Move Move { get; }

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
        public GameTreeNodeCollection Branches { get; }

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


        public GameTreeNode PreviousMove
        {
            get
            {
                GameTreeNode parent = this.Parent;
                while (parent != null && parent.Move.Kind == MoveKind.None)
                {
                    parent = parent.Parent;
                }
                return parent;
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
        /// Gets or sets a value indicating whether reaching this node means the player has successfully
        /// solved a tsumego problem.
        /// </summary>
        public bool TsumegoCorrect { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether reaching this node means that further exploration of 
        /// this branch will not yield a correct solution to a tsumego problem, and the problem should count
        /// as failed.
        /// </summary>
        public bool TsumegoWrong { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this node is part of the tsumego definition. Other nodes
        /// would be created by the player, are termed 'unexpected' and are considered wrong.
        /// </summary>
        public bool TsumegoExpected { get; set; }

        /// <summary>
        /// Gets the list of positions that are known as possible continuation from this node to the author
        /// of the tsumego problem that contains this node.
        /// </summary>
        public List<Position> TsumegoMarkedPositions { get; } = new List<Position>();

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
                throw new ArgumentNullException(nameof(node),"Node cant be null");

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

        public void FillBoardStateOfRoot(GameBoardSize boardSize, IRuleset ruleset)
        {
            if (this.Parent != null) throw new InvalidOperationException("Only call this on a root.");
            FillBoardStateInternal(new GameBoard(boardSize), ruleset);
        }

        public void FillBoardState(IRuleset ruleset)
        {
            if (this.Parent == null) throw new InvalidOperationException("Only call this on a child node.");
            FillBoardStateInternal(new GameBoard(this.Parent.BoardState), ruleset);
        }

        private void FillBoardStateInternal(GameBoard copyOfPreviousBoard, IRuleset ruleset)
        {
            foreach (var position in this.AddBlack)
                copyOfPreviousBoard[position] = StoneColor.Black;
            foreach (var position in this.AddWhite)
                copyOfPreviousBoard[position] = StoneColor.White;

           MoveProcessingResult mpr =
                ruleset.ProcessMove(copyOfPreviousBoard, this.Move, new List<GameBoard>());

          this.BoardState = mpr.NewBoard;
        }
    }
}