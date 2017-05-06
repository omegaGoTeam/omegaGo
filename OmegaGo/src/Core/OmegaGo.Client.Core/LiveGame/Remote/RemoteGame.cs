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
    }
}
