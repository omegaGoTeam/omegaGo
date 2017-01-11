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
    public abstract class GameBuilder<TGameType, TBuilderType>
        where TGameType : LiveGameBase
        where TBuilderType : GameBuilder<TGameType, TBuilderType>
    {
        private readonly TBuilderType _concreteBuilderInstance;

        private float _komi = 0.5f;
        private int _handicap = 0;
        private RulesetType _rulesetType = RulesetType.Japanese;
        private GameBoardSize _boardSize = new GameBoardSize(19);
        private CountingType? _countingType = null;
        private HandicapPlacementType _handicapPlacementType = HandicapPlacementType.Fixed;
        private int _aiStrength = 1;

        public GameBuilder()
        {
            _concreteBuilderInstance = (TBuilderType)this;
        }

        public TBuilderType SetAIStrength(int aiStrength)
        {
            if (aiStrength < 1 || aiStrength > 10) throw new ArgumentOutOfRangeException(nameof(aiStrength));
            _aiStrength = aiStrength;
            return _concreteBuilderInstance;
        }

        public TBuilderType SetKomi(float komi)
        {
            _komi = komi;
            return _concreteBuilderInstance;
        }

        public TBuilderType SetWhiteHandicap(int handicap)
        {
            _handicap = handicap;
            return _concreteBuilderInstance;
        }

        public TBuilderType SetRuleset(RulesetType rulesetType)
        {
            _rulesetType = rulesetType;
            return _concreteBuilderInstance;
        }

        public TBuilderType SetBoardSize(GameBoardSize boardSize)
        {
            _boardSize = boardSize;
            return _concreteBuilderInstance;
        }

        public TBuilderType SetCountingType(CountingType countingType)
        {
            _countingType = countingType;
            return _concreteBuilderInstance;
        }

        public TBuilderType SetHandicapPlacementType(HandicapPlacementType handicapPlacementType)
        {
            _handicapPlacementType = handicapPlacementType;
            return _concreteBuilderInstance;
        }

        public abstract TBuilderType SetWhitePlayer(GamePlayer player);

        public abstract TBuilderType SetBlackPlayer(GamePlayer player);

        protected abstract void ValidatePlayer(GamePlayer player);

        public abstract TGameType Build();
    }

    public static class GameBuilder
    {
        public static LocalGameBuilder CreateLocalGame() => new LocalGameBuilder();

        public static OnlineGameBuilder CreateOnlineGame() => new OnlineGameBuilder();
    }
}
