using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;

namespace OmegaGo.Core.Online.Common
{
    public abstract class RemoteGame : LiveGameBase
    {
        protected RemoteGame(RemoteGameInfo info) : base(info)
        {
        }
    }
}
