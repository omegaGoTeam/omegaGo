using System;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.LiveGame.Connectors
{
    /// <summary>
    /// Connects a game to a endpoint
    /// </summary>
    public interface IGameConnector
    {
        /// <summary>
        /// Indicates that return to main phase is forced
        /// </summary>
        event EventHandler LifeDeathReturnToMainForced;

        /// <summary>
        /// Indicates that undo of death marks is requested
        /// </summary>
        event EventHandler LifeDeathUndoDeathMarksRequested;

        /// <summary>
        /// Indicates that undo of death marks is forced
        /// </summary>
        event EventHandler LifeDeathUndoDeathMarksForced;

        /// <summary>
        /// Indicates that life and death is requested to be finished
        /// </summary>
        event EventHandler LifeDeathDoneRequested;

        /// <summary>
        /// Indicates that life and death is forced to be finished
        /// </summary>
        event EventHandler LifeDeathDoneForced;

        /// <summary>
        /// Indicates that a group is requested to be killed
        /// </summary>
        event EventHandler<Position> LifeDeathKillGroupRequested;

        /// <summary>
        /// Indicates that a group is forced to be killed
        /// </summary>
        event EventHandler<Position> LifeDeathKillGroupForced;

        /// <summary>
        /// Indicates that a group must be brought back to life
        /// </summary>
        event EventHandler<Position> LifeDeathRevivifyGroupForced;

        /// <summary>
        /// Indicates a request to undo a move in main phase
        /// </summary>
        event EventHandler MainUndoRequested;

        /// <summary>
        /// Indicates that undo is being forced in main phase
        /// </summary>
        event EventHandler MainUndoForced;
        
        /// <summary>
        /// Informs the connector about a performed move
        /// </summary>
        void MovePerformed(Move move);
    }
}
