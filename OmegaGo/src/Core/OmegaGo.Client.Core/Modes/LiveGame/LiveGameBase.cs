using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame
{
    public abstract class LiveGameBase : IMode
    {
        IGameController Controller { get; }

        GameInfo Info { get; }
    }
}
