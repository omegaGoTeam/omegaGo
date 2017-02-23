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
        /// Informs the connector about a performed move
        /// </summary>
        /// <param name="move">Move</param>
        void MovePerformed(Move move);
    }
}
