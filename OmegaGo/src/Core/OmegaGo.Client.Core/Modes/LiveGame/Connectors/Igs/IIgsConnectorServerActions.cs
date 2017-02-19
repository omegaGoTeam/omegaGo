using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Connectors.Igs
{
    internal interface IIgsConnectorServerActions
    {
        /// <summary>
        /// Handles incoming move from the server
        /// </summary>
        /// <param name="moveIndex">Index of the move</param>
        /// <param name="move">Move</param>
        void MoveFromServer(int moveIndex, Move move);

        /// <summary>
        /// A undo operation is coming from the server
        /// </summary>
        void UndoFromServer();

        /// <summary>
        /// Sets the game's handicap
        /// </summary>
        /// <param name="stoneCount">Number of handicap stones</param>
        void HandicapFromServer(int stoneCount);

        /// <summary>
        /// Receives and handles resignation from server
        /// </summary>
        /// <param name="resigningPlayerColor">Color of the resigning player</param>
        void ResignationFromServer( StoneColor resigningPlayerColor );
    }
}