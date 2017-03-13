using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame
{
    /// <summary>
    /// Base interface for a live game
    /// </summary>
    public interface IGame 
    {
        /// <summary>
        /// Controller of the live game
        /// </summary>
        IGameController Controller { get; }

        /// <summary>
        /// Info about the game
        /// </summary>
        GameInfo Info { get; }
    }
}
