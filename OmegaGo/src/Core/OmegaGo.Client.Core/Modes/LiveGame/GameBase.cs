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
    public abstract class GameBase : IGame
    {
        protected GameBase(GameInfo info)
        {
            Info = info;
        }

        /// <summary>
        /// Controller of the game
        /// </summary>
        public abstract IGameController Controller { get; }

        /// <summary>
        /// Info about the game
        /// </summary>
        public GameInfo Info { get; }
    }
}