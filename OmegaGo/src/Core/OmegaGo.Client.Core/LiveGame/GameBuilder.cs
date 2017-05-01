using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Igs;
using IgsGameBuilder = OmegaGo.Core.Modes.LiveGame.Remote.Igs.IgsGameBuilder;

namespace OmegaGo.Core.Modes.LiveGame
{
    public abstract class GameBuilder<TGameType, TBuilderType>
        where TGameType : IGame
        where TBuilderType : GameBuilder<TGameType, TBuilderType>
    {
        private readonly TBuilderType _concreteBuilderInstance;

        private float _komi = 0.5f;
        private int _handicap;
        private RulesetType _rulesetType = RulesetType.Japanese;
        private GameBoardSize _boardSize = new GameBoardSize(19);
        private CountingType _countingType = Rules.CountingType.Area;
        private HandicapPlacementType _handicapPlacementType = Phases.HandicapPlacement.HandicapPlacementType.Fixed;

        private GamePlayer _whitePlayer;
        private GamePlayer _blackPlayer;

        protected GameBuilder()
        {
            _concreteBuilderInstance = (TBuilderType)this;
        }

        public TBuilderType Komi(float komi)
        {
            _komi = komi;
            return _concreteBuilderInstance;
        }

        public TBuilderType Handicap(int handicap)
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

        /// <summary>
        /// This should validate the player for the concrete builder
        /// </summary>
        /// <param name="player">Player to validate</param>
        /// <returns>Is player valid?</returns>
        protected abstract bool ValidatePlayer(GamePlayer player);

        /// <summary>
        /// Builds the game
        /// </summary>
        /// <returns>Built game</returns>
        public abstract TGameType Build();

        /// <summary>
        /// Creates the ruleset
        /// </summary>
        /// <returns>Ruleset instance</returns>
        protected IRuleset CreateRuleset()
        {
            return Rules.Ruleset.Create(_rulesetType, _boardSize, _countingType);
        }

        /// <summary>
        /// Creates the game info
        /// </summary>
        /// <returns>Game info</returns>
        protected GameInfo CreateGameInfo()
        {
            return new GameInfo(_whitePlayer.Info, _blackPlayer.Info, _boardSize, _rulesetType, _handicap,
                _handicapPlacementType, _komi, _countingType);
        }

        /// <summary>
        /// Creates game players
        /// </summary>
        /// <returns>Player pair</returns>
        protected PlayerPair CreatePlayers()
        {
            return new PlayerPair(_blackPlayer, _whitePlayer);
        }

        /// <summary>
        /// Internal player setter
        /// </summary>
        /// <param name="player">Player</param>
        /// <param name="color">Color of the player</param>
        private void SetPlayerCore(GamePlayer player, StoneColor color)
        {
            if (color == StoneColor.None) throw new ArgumentOutOfRangeException(nameof(color), "Color must be valid for a player");
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (player.Info.Color != color) throw new ArgumentException("The provided player color doesn't match expectation.", nameof(player));
            if (!ValidatePlayer(player)) throw new ArgumentException("The provided player is not valid for this type of game", nameof(player));
            if (color == StoneColor.White) _whitePlayer = player;
            if (color == StoneColor.Black) _blackPlayer = player;
        }
    }

    public static class GameBuilder
    {
        public static LocalGameBuilder CreateLocalGame() => new LocalGameBuilder();

        public static IgsGameBuilder CreateOnlineGame(IgsGameInfo ogi) => new IgsGameBuilder(ogi);
    }
}
