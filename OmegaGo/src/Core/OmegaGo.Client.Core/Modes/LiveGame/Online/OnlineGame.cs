using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class OnlineGame : LiveGameBase
    {
        public OnlineGame(OnlineGameInfo info) : base(info)
        {
            Metadata = info;
        }

        public OnlineGameInfo Metadata { get; }

        public override IGameController Controller { get; }
    }
}
