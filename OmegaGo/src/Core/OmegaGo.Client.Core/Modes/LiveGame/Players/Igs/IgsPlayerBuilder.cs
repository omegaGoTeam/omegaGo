using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Igs
{
    public sealed class IgsPlayerBuilder : PlayerBuilder<GamePlayer, IgsPlayerBuilder>
    {
        public IgsPlayerBuilder(StoneColor color) : base(color)
        {
        }

        public override GamePlayer Build()
        {
            throw new NotImplementedException();
        }
    }
}
