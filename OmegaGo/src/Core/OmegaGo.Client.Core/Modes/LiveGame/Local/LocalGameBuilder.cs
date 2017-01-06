using OmegaGo.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Local
{
    public class LocalGameBuilder : GameBuilder<LocalGame, LocalGameBuilder>
    {
        protected override LocalGameBuilder DerivedThis => this;

        public override LocalGame Build()
        {
            throw new NotImplementedException();
        }

        public LocalGameBuilder SetAIStrength( AIStrength strength)
        {

            return this;
        }

        public LocalGameBuilder SetOpponent( LocalGameOpponent opponent)
        {

            return this;
        }
    }
}
