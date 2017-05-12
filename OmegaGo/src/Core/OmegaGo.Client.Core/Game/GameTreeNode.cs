using System;
using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Game.GameTreeNodeData;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Represents a node in a game tree. In games, the game tree will be a "path" of <see cref="GameTreeNode" />s
    /// with each node having only a single child. In game analysis, the game tree may be an actual tree.
    /// </summary>
    public sealed class GameTreeNode
    {
        private readonly Dictionary<Type, object> _additionalNodeInfo = new Dictionary<Type, object>();
        private readonly MarkupInfo _markups = new MarkupInfo();

        public GameTreeNode(Move move = null)
        {
            //none move is default
            if ( move == null ) move = Move.NoneMove;           
            Branches = new GameTreeNodeCollection(this);
            Move = move;
        }

        // Information taken from official SGF file definition
        // http://www.red-bean.com/sgf/proplist_ff.html
        // and SGF file examples
        // http://www.red-bean.com/sgf/examples/

        public MarkupInfo Markups => _markups;
        public string Comment { get; set; }
        public string Name { get; set; }

        public List<Position> AddBlack { get; set; } = new List<Position>();
        public List<Position> AddWhite { get; set; } = new List<Position>();

        /// <summary>
        /// Describes current state of the entire game board. Can be null.
        /// </summary>
        public GameBoard BoardState { get; set; }

        /// <summary>
        /// Describes the state of stone groups on the game board.
        /// </summary>
        public GroupState GroupState { get; set; }

        // Contain territory
        // public List<Shape> Figures { get; set; } - Implement Shape 
        public List<KeyValuePair<Position, string>> Labels { get; set; }

        /// <summary>
        /// Gets or sets the move that caused this <see cref="GameTreeNode" /> to exist.
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
                var number = 0;
                var node = this;
                while (node != null)
                {
                    node = node.Parent;
                    number++;
                }
                return number;
            }
        }

        /// <summary>
        /// Gets the first child node of this node, if it exists, otherwise null. 
        /// </summary>
        public GameTreeNode NextNode
        {
            get
            {
                switch (Branches.Count)
                {
                    case 0:
                        return null;
                    default:
                        return Branches[0];
                }
            }
        }


        public GameTreeNode PreviousMove
        {
            get
            {
                var parent = Parent;
                while (parent != null && parent.Move.Kind == MoveKind.None)
                    parent = parent.Parent;
                return parent;
            }
        }

        public IEnumerable<GameTreeNode> GetTimelineView
        {
            get
            {
                yield return this;
                var node = NextNode;
                while (node != null)
                {
                    yield return node;
                    node = node.NextNode;
                }
            }
        }
        
        public TsumegoNodeInfo Tsumego { get; } = new TsumegoNodeInfo();
        public PrisonersNodeInfo Prisoners { get; } = new PrisonersNodeInfo();

        /// <summary>
        /// Gets the list of all moves that lead to the provided node.
        /// The list is starting with root node.
        /// </summary>
        /// <param name="filterNonMoves">determines whether nodes with MoveKind.None should be included</param>
        /// <returns>nodes history</returns>
        public IEnumerable<GameTreeNode> GetNodeHistory( bool filterNonMoves = true )
        {
            var nodeHistory = new List<GameTreeNode>();
            var currentNode = this;
            do
            {
                var isMoveNode = (currentNode.Move.Kind == MoveKind.Pass || currentNode.Move.Kind == MoveKind.PlaceStone);
                if (!filterNonMoves || isMoveNode) { 
                    nodeHistory.Insert(0, currentNode);
                }                   
                currentNode = currentNode.Parent;
            } while (currentNode != null);

            return nodeHistory;
        }

        /// <summary>
        /// Returns the list of all preceding game boards including current node
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GameBoard> GetGameBoardHistory()
        {
            var nodes = GetNodeHistory();
            var boards = nodes.Select(node => node.BoardState).ToList();
            return boards;
        }

        /// <summary>
        /// Removes the given child of node.
        /// </summary>
        /// <param name="child">Child to remove</param>
        public void RemoveChild(GameTreeNode child)
        {
            if (!Branches.RemoveNode(child))
             throw new InvalidOperationException("Cannot remove child: The given node is not a child of current node"); 
        
        }

        /// <summary>
        /// Gets node info of a given type. Throws <see cref="KeyNotFoundException"/> if info is not found.
        /// </summary>
        /// <typeparam name="T">Type of info to retrieve</typeparam>
        /// <returns>Node info</returns>
        public T GetNodeInfo<T>()
        {
            return (T) _additionalNodeInfo[typeof(T)];
        }

        /// <summary>
        /// Gets or creates node info for types with parameterless constructor
        /// </summary>
        /// <typeparam name="T">Type of info to retrieve</typeparam>
        /// <returns>Node info</returns>
        public T GetOrCreateNodeInfo<T>() where T : new()
        {
            var type = typeof(T);
            if (!_additionalNodeInfo.ContainsKey(type))
                _additionalNodeInfo[type] = new T();
            return (T) _additionalNodeInfo[type];
        }

        /// <summary>
        /// Gets or creates node info. If node info does not exist, it is created using
        /// the provided Functor
        /// </summary>
        /// <typeparam name="T">Type of info to retrieve</typeparam>
        /// <param name="defaultValue">Default value builder</param>
        /// <returns>Node info</returns>
        public T GetOrCreateNodeInfo<T>(Func<T> defaultValue)
        {
            var type = typeof(T);
            if (!_additionalNodeInfo.ContainsKey(type))
                _additionalNodeInfo[type] = defaultValue();
            return (T) _additionalNodeInfo[type];
        }

        /// <summary>
        /// Stores node info
        /// </summary>
        /// <typeparam name="T">Type of info to store</typeparam>
        /// <param name="nodeInfo">Node info to store</param>
        public void SetNodeInfo<T>(T nodeInfo)
        {
            _additionalNodeInfo[typeof(T)] = nodeInfo;
        }

        public void FillBoardStateOfRoot(GameBoardSize boardSize, IRuleset ruleset)
        {
            if (Parent != null) throw new InvalidOperationException("Only call this on a root.");
            FillBoardStateInternal(new GameBoard(boardSize), new GroupState(ruleset.RulesetInfo), ruleset);
        }

        public void FillBoardState(IRuleset ruleset)
        {
            if (Parent == null) throw new InvalidOperationException("Only call this on a child node.");
            FillBoardStateInternal(new GameBoard(Parent.BoardState), new GroupState(Parent.GroupState, ruleset.RulesetInfo), ruleset);
        }

        private void FillBoardStateInternal(GameBoard copyOfPreviousBoard, GroupState copyOfPreviousGroupState, IRuleset ruleset)
        {
            foreach (var position in AddBlack)
                copyOfPreviousBoard[position] = StoneColor.Black;
            foreach (var position in AddWhite)
                copyOfPreviousBoard[position] = StoneColor.White;

            BoardState = copyOfPreviousBoard;

            if (AddBlack.Count == 0 && AddWhite.Count == 0)
            {
                GroupState = copyOfPreviousGroupState;
            }
            else
            {
                ruleset.RulesetInfo.SetState(copyOfPreviousBoard);
                GroupState = ruleset.RulesetInfo.GroupState;
            }

            //process only if move was performed
            if (Move.Kind != MoveKind.None)
            {
                var processMove = ruleset.ProcessMove(this, Move);
                BoardState = processMove.NewBoard;
                GroupState = processMove.NewGroupState;
                Move.Captures.AddRange(processMove.Captures);
            }
        }

        public override string ToString()
        {
            return Move + "/" + BoardState;
        }
    }
}