using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Local
{
    public class LocalGame : LiveGameBase
    {
        public LocalGame(IGameController controller, GameInfo info) : base(controller, info)
        {
        }
    }
}
