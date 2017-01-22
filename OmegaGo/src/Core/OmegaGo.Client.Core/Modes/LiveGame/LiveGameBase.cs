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
        protected LiveGameBase(IGameController controller, GameInfo info)
        {
            Controller = controller;
            Info = info;
        }

        IGameController Controller { get; }

        GameInfo Info { get; }

        GameState State { get; }
    }
}