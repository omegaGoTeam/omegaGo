using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.LiveGame.Remote;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.Core.Modes.LiveGame.Remote
{
    public interface IRemoteGame : IGame
    {
        /// <summary>
        /// Game info
        /// </summary>
        new RemoteGameInfo Info { get; }
    }
}
