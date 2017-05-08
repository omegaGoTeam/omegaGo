using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Time;
using OmegaGo.Core.Time.None;

namespace OmegaGo.Core.Modes.LiveGame.Players.Builders
{
    public abstract class PlayerBuilder<TPlayerType, TBuilderType>
        where TPlayerType : GamePlayer
        where TBuilderType : PlayerBuilder<TPlayerType, TBuilderType>
    {
        /// <summary>
        /// Strongly typed builder instance
        /// </summary>
        private readonly TBuilderType _concreteBuilderInstance;

        /// <summary>
        /// Color of the player
        /// </summary>
        protected readonly StoneColor Color;

        /// <summary>
        /// Player's name
        /// </summary>
        private string _name = "";

        /// <summary>
        /// Player's rank
        /// </summary>
        private string _rank = "";

        /// <summary>
        /// Time clock for the player
        /// </summary>
        protected TimeControl TimeClock { get; private set; } = new NoTimeControl();

        /// <summary>
        /// Creates a player builder
        /// </summary>
        /// <param name="color">Stone color of the player</param>
        protected PlayerBuilder(StoneColor color)
        {
            if (!Enum.IsDefined(typeof(StoneColor), color) || color == StoneColor.None)
                throw new ArgumentOutOfRangeException(nameof(color), "Player color not valid");
            Color = color;
            _concreteBuilderInstance = (TBuilderType)this;
        }

        /// <summary>
        /// Sets the name
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Builder</returns>
        public TBuilderType Name(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            _name = name;
            return _concreteBuilderInstance;
        }

        /// <summary>
        /// Sets the name
        /// </summary>
        /// <param name="clock">Clock</param>
        /// <returns>Builder</returns>
        public TBuilderType Clock(TimeControl clock)
        {
            if (clock == null) throw new ArgumentNullException(nameof(clock));
            TimeClock = clock;
            return _concreteBuilderInstance;
        }

        /// <summary>
        /// Sets the rank
        /// </summary>
        /// <param name="rank">Rank</param>
        /// <returns>Builder</returns>
        public TBuilderType Rank(string rank)
        {
            if (rank == null) throw new ArgumentNullException(nameof(rank));
            _rank = rank;
            return _concreteBuilderInstance;
        }
        

        /// <summary>
        /// Builds the player
        /// </summary>
        /// <returns>Player instance</returns>
        public abstract TPlayerType Build();

        /// <summary>
        /// Creates player info
        /// </summary>
        /// <returns>Player info instance</returns>
        protected PlayerInfo CreatePlayerInfo() => new PlayerInfo(Color, _name, _rank);
    }
}
