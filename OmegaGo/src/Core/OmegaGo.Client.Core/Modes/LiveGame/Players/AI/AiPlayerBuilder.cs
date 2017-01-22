using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.AI
{
    public sealed class AiPlayerBuilder : PlayerBuilder<AiPlayer, AiPlayerBuilder>
    {
        public AiPlayerBuilder(StoneColor color) : base(color)
        {
        }

        public override AiPlayer Build()
        {
            throw new NotImplementedException();
        }
    }
}
