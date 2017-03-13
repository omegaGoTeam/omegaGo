using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame
{
    /// <summary>
    /// Base abstract class for a game
    /// </summary>
    public abstract class GameBase<TGameInfo, TGameController> : IGame
        where TGameInfo : GameInfo
        where TGameController : IGameController
    {
        /// <summary>
        /// Creates an instance of a game
        /// </summary>
        /// <param name="info">Game info for the game</param>
        protected GameBase(TGameInfo info)
        {
            Info = info;
        }

        /// <summary>
        /// Controller of the game
        /// </summary>
        public abstract TGameController Controller { get; }
        
        /// <summary>
        /// Info about the game
        /// </summary>
        public TGameInfo Info { get; }

        /// <summary>
        /// Explicit implementation of IGame.Controller
        /// </summary>
        IGameController IGame.Controller => Controller;

        /// <summary>
        /// Explicit implementation of IGame.Info
        /// </summary>
        GameInfo IGame.Info => Info;
    }
}