using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Remote;

namespace OmegaGo.Core.Online.Common
{
    /// <summary>
    /// A common base class for games which run on an online server.
    /// </summary>
    public abstract class RemoteGame<TRemoteGameInfo, TGameController> : GameBase<TRemoteGameInfo, TGameController>, IRemoteGame
        where TRemoteGameInfo : RemoteGameInfo
        where TGameController : IGameController
    {
        /// <summary>
        /// Creates a remote game
        /// </summary>
        /// <param name="info">Remote game info</param>
        protected RemoteGame(TRemoteGameInfo info) : base(info)
        {
            
        }

        /// <summary>
        /// Explicit implementation of IRemoteGame.Info
        /// </summary>
        RemoteGameInfo IRemoteGame.Info => Info;
    }
}
