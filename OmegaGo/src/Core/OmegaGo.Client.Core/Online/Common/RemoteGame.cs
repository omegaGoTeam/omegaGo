using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;

namespace OmegaGo.Core.Online.Common
{
    /// <summary>
    /// A common base class for games which run on an online server.
    /// </summary>
    public abstract class RemoteGame : GameBase
    {
        protected RemoteGame(RemoteGameInfo info) : base(info)
        {
            
        }
    }
}
