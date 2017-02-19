using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.Core.Modes.LiveGame.Remote
{
    public interface IRemoteGame : IGame
    {
        new RemoteGameInfo Info { get; }
    }
}
