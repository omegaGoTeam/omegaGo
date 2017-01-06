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
    public abstract class GameBuilder<GameType, BuilderType>
        where GameType : LiveGameBase
        where BuilderType : GameBuilder<GameType, BuilderType>
    {
        protected abstract BuilderType DerivedThis { get; }

        public BuilderType SetKomi(float komi)
        {

            return DerivedThis;
        }

        public BuilderType SetWhiteHandicap()
        {
            return DerivedThis;
        }

        public BuilderType SetRuleset(RulesetType rulesetType)
        {
            return DerivedThis;
        }

        public BuilderType SetBoardSize(GameBoardSize boardSize)
        {
            return DerivedThis;
        }

        public BuilderType SetCountingType(CountingType countingType)
        {
            return DerivedThis;
        }

        public BuilderType SetHandicapPlacementType(HandicapPlacementType handicapPlacementType)
        {
            return DerivedThis;
        }

        public abstract GameType Build();
    }

    public static class GameBuilder
    {
        public static LocalGameBuilder LocalGame() => new LocalGameBuilder();

        public static OnlineGameBuilder OnlineGame() => new OnlineGameBuilder();
    }
}
