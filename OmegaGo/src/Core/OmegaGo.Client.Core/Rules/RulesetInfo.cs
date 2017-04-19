using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    /// <summary>
    /// Represents the current state of board and groups on the board.
    /// </summary>
    internal class RulesetInfo : IRulesetInfo
    {
        /// <summary>
        /// Initializes the state of ruleset.
        /// </summary>
        /// <param name="gbSize">Size of game board.</param>
        /// <param name="currentBoard">The state of game board.</param>
        /// <param name="groupState">The state of groups.</param>
        internal RulesetInfo(GameBoardSize gbSize)
        {
            BoardSize = gbSize;
            BoardState = new GameBoard(gbSize);
            GroupState = new GroupState(this);
        }

        /// <summary>
        /// The size of game board.
        /// </summary>
        public GameBoardSize BoardSize { get; }

        /// <summary>
        /// The current state of game board.
        /// </summary>
        public GameBoard BoardState { get; private set; }

        /// <summary>
        /// The current state of groups on the game board.
        /// See <see cref="GroupState"/>.
        /// </summary>
        public GroupState GroupState { get; private set; }

        /// <summary>
        /// Updates Ruleset state according to given game board state.
        /// </summary>
        /// <param name="currentBoard">State of board.</param>
        public void SetState(GameBoard currentBoard)
        {
            BoardState = new GameBoard(currentBoard);
            GroupState = new GroupState(this);
            GroupState.FillGroupMap(currentBoard);
            GroupState.CountLiberties();
        }

        public void SetState(GameBoard board, GroupState groupState)
        {
            BoardState = board;
            GroupState = groupState;
        }

        public void SetBoard(GameBoard board)
        {
            BoardState = new GameBoard(board);
        }


    }
}
