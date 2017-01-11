using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Players
{
    public abstract class PlayerBuilder<TPlayerType, TBuilderType>
        where TPlayerType : GamePlayer
        where TBuilderType : PlayerBuilder<TPlayerType, TBuilderType>    
    {
        public abstract void Build();
    }
}
