using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame
{
    public abstract class LiveGameBase : ILiveGame
    {
        protected LiveGameBase( GameInfo info)
        {
            Info = info;
        }

        public abstract IGameController Controller { get; }

        public GameInfo Info { get; }
    }
}