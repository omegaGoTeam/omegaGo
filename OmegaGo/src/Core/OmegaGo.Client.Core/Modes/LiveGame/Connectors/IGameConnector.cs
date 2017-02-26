using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Connectors
{
    /// <summary>
    /// Connects a game to a endpoint
    /// </summary>
    public interface IGameConnector
    {
        /// <summary>
        /// Indicates that return to main phase is forced
        /// </summary>
        event EventHandler LifeDeathForceReturnToMain;

        /// <summary>
        /// Indicates that undo of death marks is requested
        /// </summary>
        event EventHandler LifeDeathRequestUndoDeathMarks;

        /// <summary>
        /// Indicates that undo of death marks is forced
        /// </summary>
        event EventHandler LifeDeathForceUndoDeathMarks;

        /// <summary>
        /// Indicates that life and death is requested to be finished
        /// </summary>
        event EventHandler LifeDeathRequestDone;

        /// <summary>
        /// Indicates that life and death is forced to be finished
        /// </summary>
        event EventHandler LifeDeathForceDone;

        /// <summary>
        /// Indicates that a group is requested to be killed
        /// </summary>
        event EventHandler<Position> LifeDeathRequestKillGroup;

        /// <summary>
        /// Indicates that a group is forced to be killed
        /// </summary>
        event EventHandler<Position> LifeDeathForceKillGroup;

        /// <summary>
        /// Indicates a request to undo a move in main phase
        /// </summary>
        event EventHandler MainRequestUndo;

        /// <summary>
        /// Indicates that undo is being forced in main phase
        /// </summary>
        event EventHandler MainForceUndo;

        /// <summary>
        /// Informs the connector about a performed move
        /// </summary>
        /// <param name="move">Move</param>
        void MovePerformed(Move move);
    }
}
