using OmegaGo.Core.Game;

namespace OmegaGo.Core.Rules
{
    public interface IRulesetInfo
    {
        /// <summary>
        /// The size of game board.
        /// </summary>
        GameBoardSize BoardSize { get; }

        /// <summary>
        /// The current state of game board.
        /// </summary>
        GameBoard BoardState { get; }

        /// <summary>
        /// The current state of groups on the game board.
        /// See <see cref="GroupState"/>.
        /// </summary>
        GroupState GroupState { get; }

        /// <summary>
        /// Determines the group state based on the given board state.
        /// </summary>
        /// <param name="board">Board state</param>
        void SetState(GameBoard board);

        /// <summary>
        /// Sets the group state and board state.
        /// </summary>
        /// <param name="board">Board state</param>
        /// <param name="groupState">Group state</param>
        void SetState(GameBoard board, GroupState groupState);

        /// <summary>
        /// Sets the board state. The group state remains unchanged. 
        /// </summary>
        /// <param name="board">Board state</param>
        void SetBoard(GameBoard board);
        
    }
}
