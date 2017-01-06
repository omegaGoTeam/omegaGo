using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame
{
    public abstract class GameBuilder<T> where T : GameBuilder<T>
    {
        protected abstract T DerivedThis { get; }

        public T SetKomi(float komi)
        {

            return DerivedThis;
        }

        public T SetWhiteHandicap()
        {
            return DerivedThis;
        }

        public T SetRuleset(RulesetType rulesetType)
        {
            return DerivedThis;
        }

        public T SetBoardSize(GameBoardSize boardSize)
        {
            return DerivedThis;
        }

        public T SetHandicapPlacementType(HandicapPlacementType handicapPlacementType)
        {
            return DerivedThis;
        }

        public abstract LiveGameBase Build();
    }

    public static class GameBuilder
    {
        public static LocalGameBuilder LocalGame() => new LocalGameBuilder();

        public static OnlineGameBuilder OnlineGame() => new OnlineGameBuilder();
    }
}
