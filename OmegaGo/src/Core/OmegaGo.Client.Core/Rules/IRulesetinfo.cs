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

        void SetState(GameBoard board);

        void SetState(GameBoard board, GroupState groupState);

        void SetBoard(GameBoard board);
        
    }
}
