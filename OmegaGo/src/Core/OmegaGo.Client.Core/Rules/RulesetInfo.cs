using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Represents the current state of board and groups on the board.
    /// </summary>
    internal class RulesetInfo
    {
        /// <summary>
        /// The size of game board.
        /// </summary>
        private static GameBoardSize _gbSize;
        
        /// <summary>
        /// The size of game board.
        /// </summary>
        internal static GameBoardSize BoardSize
        {
            get { return _gbSize; }
        }

        /// <summary>
        /// Komi compensation.
        /// </summary>
        internal static float Komi { get; set; }

        /// <summary>
        /// The current state of game board.
        /// </summary>
        internal static GameBoard BoardState { get; set; }

        /// <summary>
        /// The current state of groups on the game board.
        /// See <see cref="GroupState"/>.
        /// </summary>
        internal static GroupState GroupState { get; set; }

        /// <summary>
        /// Initializes the state of ruleset.
        /// </summary>
        /// <param name="gbSize">Size of game board.</param>
        /// <param name="currentBoard">The state of game board.</param>
        /// <param name="groupState">The state of groups.</param>
        internal RulesetInfo(GameBoardSize gbSize, GameBoard currentBoard, GroupState groupState)
        {
            _gbSize = gbSize;
            BoardState = currentBoard;
            GroupState = groupState;
        }
    }
}
