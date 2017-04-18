using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Online.Igs.Events;

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

        /// <summary>
        /// Server indicates that it wants to change the game phase
        /// </summary>
        /// <param name="gamePhase">Game phase type to start</param>
        void SetPhaseFromServer(GamePhaseType gamePhase);

        void TimeControlAdjustment(IgsTimeControlAdjustmentEventArgs igsTimeControlAdjustmentEventArgs);
        void ForceLifeDeathKillGroup(Position deadPosition);
        void ForceLifeDeathUndoDeathMarks();
        void ForceMainUndo();
    }
}