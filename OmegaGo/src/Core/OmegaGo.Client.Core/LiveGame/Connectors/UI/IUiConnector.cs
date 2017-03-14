using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Connectors.UI
{
    /// <summary>
    /// UI actions
    /// </summary>
    public interface IUiConnectorActions
    {
        /// <summary>
        /// Makes a move for the player on turn
        /// </summary>
        /// <param name="position">Position to play</param>
        void MakeMove(Position position);
        
        /// <summary>
        /// Resigns the player
        /// </summary>
        void Resign();

        /// <summary>
        /// Makes the player pass
        /// </summary>
        void Pass();

        /// <summary>
        /// AI log
        /// </summary>
        event EventHandler<string> AiLog;
    }
}
