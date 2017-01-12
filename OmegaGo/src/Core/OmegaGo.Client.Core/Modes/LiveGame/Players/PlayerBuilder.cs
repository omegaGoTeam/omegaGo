using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players
{
    public abstract class PlayerBuilder<TPlayerType, TBuilderType>
        where TPlayerType : GamePlayer
        where TBuilderType : PlayerBuilder<TPlayerType, TBuilderType>
    {
        protected abstract GamePlayerType PlayerType { get; }

        public PlayerBuilder(StoneColor color)
        {

        }

        public abstract void Build();
    }
}
