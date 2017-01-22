using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;

namespace OmegaGo.Core.Modes.LiveGame.Players.Local
{
    public sealed class HumanPlayerBuilder : PlayerBuilder<HumanPlayer, HumanPlayerBuilder>
    {
        public HumanPlayerBuilder(StoneColor color) : base(color)
        {
        }

        public override HumanPlayer Build()
        {
            throw new NotImplementedException();
            //return new HumanPlayer( CreatePlayerInfo(), new ObsoleteLocalAgent());
        }
    }
}
