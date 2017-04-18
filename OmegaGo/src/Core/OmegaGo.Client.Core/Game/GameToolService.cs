using System;
using OmegaGo.Core.Game.Tools;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game
{
    public sealed class GameToolServices : IToolServices
    {
        private readonly Ruleset _ruleset;
        private readonly GameTree _gameTree;

        private GameTreeNode _currentNode;
        private Position _pointerOverPosition;

        /// <summary>
        /// Initializes a new instance of GameToolServices.
        /// </summary>
        /// <param name="ruleset">a ruleset that should be used for providing ruleset services</param>
        /// <param name="gameTree">a game tree representing the current game</param>
        public GameToolServices(Ruleset ruleset, GameTree gameTree)
        {
            _ruleset = ruleset;
            _gameTree = gameTree;
        }

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
    }
}
