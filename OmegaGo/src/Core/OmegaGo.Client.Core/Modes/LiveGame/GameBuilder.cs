using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;

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
        private CountingType _countingType = Rules.CountingType.Area;
        private HandicapPlacementType _handicapPlacementType = Rules.HandicapPlacementType.Fixed;
        private int _aiStrength = 1;

        private GamePlayer _whitePlayer = null;
        private GamePlayer _blackPlayer = null;

        public GameBuilder()
        {
            _concreteBuilderInstance = (TBuilderType)this;
        }

        public TBuilderType AIStrength(int aiStrength)
        {
            if (aiStrength < 1 || aiStrength > 10) throw new ArgumentOutOfRangeException(nameof(aiStrength));
            _aiStrength = aiStrength;
            return _concreteBuilderInstance;
        }

        public TBuilderType Komi(float komi)
        {
            _komi = komi;
            return _concreteBuilderInstance;
        }

        public TBuilderType WhiteHandicap(int handicap)
        {
            _handicap = handicap;
            return _concreteBuilderInstance;
        }

        public TBuilderType Ruleset(RulesetType rulesetType)
        {
            _rulesetType = rulesetType;
            return _concreteBuilderInstance;
        }

        public TBuilderType BoardSize(GameBoardSize boardSize)
        {
            _boardSize = boardSize;
            return _concreteBuilderInstance;
        }

        public TBuilderType CountingType(CountingType countingType)
        {
            _countingType = countingType;
            return _concreteBuilderInstance;
        }

        public TBuilderType HandicapPlacementType(HandicapPlacementType handicapPlacementType)
        {
            _handicapPlacementType = handicapPlacementType;
            return _concreteBuilderInstance;
        }

        public void SetPlayerCore(GamePlayer player, StoneColor color)
        {
            if (color == StoneColor.None) throw new ArgumentOutOfRangeException(nameof(color), "Color must be valid for a player");
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (player.Info.Color != color) throw new ArgumentException("The provided player color doesn't match expectation.");
            ValidatePlayer(player);
            if (color == StoneColor.White) _whitePlayer = player;
            if (color == StoneColor.Black) _blackPlayer = player;
        }

        public TBuilderType BlackPlayer(GamePlayer player)
        {
            SetPlayerCore(player, StoneColor.Black);
            return _concreteBuilderInstance;
        }

        /// <summary>
        /// Sets the game's white player
        /// </summary>
        /// <param name="player">Player</param>
        /// <returns>Builder</returns>
        public TBuilderType WhitePlayer(GamePlayer player)
        {
            SetPlayerCore(player, StoneColor.White);
            return _concreteBuilderInstance;
        }

        protected abstract void ValidatePlayer(GamePlayer player);

        public abstract TGameType Build();

        /// <summary>
        /// Creates the ruleset
        /// </summary>
        /// <returns>Ruleset instance</returns>
        protected IRuleset CreateRuleset()
        {
            return Rules.Ruleset.Create(_rulesetType, _boardSize, _countingType);
        }
    }

    public static class GameBuilder
    {
        public static LocalGameBuilder CreateLocalGame() => new LocalGameBuilder();

        public static OnlineGameBuilder CreateOnlineGame() => new OnlineGameBuilder();
    }
}
