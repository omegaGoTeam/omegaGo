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
        public OnlineGame(GameInfo info) : base(info)
        {
        }

        public override IGameController Controller { get; }
    }
}
