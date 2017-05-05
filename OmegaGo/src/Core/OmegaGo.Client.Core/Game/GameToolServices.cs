using System;
using OmegaGo.Core.Game.Tools;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game
{
    public sealed class GameToolServices : IToolServices
    {
        private readonly IRuleset _ruleset;
        private readonly GameTree _gameTree;

        private GameTreeNode _currentNode;
        private Position _pointerOverPosition;
        
        /// <summary>
        /// Initializes a new instance of GameToolServices.
        /// </summary>
        /// <param name="ruleset">a ruleset that should be used for providing ruleset services</param>
        /// <param name="gameTree">a game tree representing the current game</param>
        public GameToolServices(IRuleset ruleset, GameTree gameTree)
        {
            if (ruleset == null)
                ruleset = Rules.Ruleset.Create(RulesetType.Chinese, gameTree.BoardSize);

            _ruleset = ruleset;
            _gameTree = gameTree;
        }

        public event EventHandler<GameTreeNode> NodeChanged;
        public event EventHandler<Position> PointerPositionChanged;
        public event EventHandler PassSoundShouldBePlayed;
        public event EventHandler StonePlacementShouldBePlayed;
        public event EventHandler StoneCapturesShouldBePlayed;

        /// <summary>
        /// Gets the active ruleset for the current game.
        /// </summary>
        public IRuleset Ruleset
        {
            get { return _ruleset; }
        }

        /// <summary>
        /// Gets the GameTree representing the current game.
        /// </summary>
        public GameTree GameTree
        {
            get { return _gameTree; }
        }

        /// <summary>
        /// Gets or sets the current game node on which the tools should operate.
        /// </summary>
        public GameTreeNode Node
        {
            get { return _currentNode; }
            set { _currentNode = value; }
        }

        /// <summary>
        /// Gets or sets the board coordinates of the current pointer position.
        /// </summary>
        public Position PointerOverPosition
        {
            get { return _pointerOverPosition; }
            set { _pointerOverPosition = value; }
        }

        ////////
        //
        //  IToolServices public API implementation
        //  
        //  - this is intended to be used only by ITools.
        //////// 
        
        public void SetNode(GameTreeNode node)
        {
            if (node == null)
                throw new ArgumentException($"{nameof(node)} cant be null");

            Node = node;
            NodeChanged?.Invoke(this, node);
        }

        public void SetPointerPosition(Position position)
        {
            if(position == null)
                throw new ArgumentException($"{nameof(position)} cant be null");

            PointerOverPosition = position;
            PointerPositionChanged?.Invoke(this, position);
        }

        public void PlayPassSound()
        {
            PassSoundShouldBePlayed?.Invoke(this, EventArgs.Empty);
        }

        public void PlayStonePlacementSound(bool wereThereAnyCaptures)
        {
            if (wereThereAnyCaptures)
            {
                StoneCapturesShouldBePlayed?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                StonePlacementShouldBePlayed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
