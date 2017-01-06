using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class OnlineGameBuilder : GameBuilder<OnlineGameBuilder>
    {
        protected override OnlineGameBuilder DerivedThis => this;

        public override LiveGameBase Build()
        {
            throw new NotImplementedException();
        }
    }
}
