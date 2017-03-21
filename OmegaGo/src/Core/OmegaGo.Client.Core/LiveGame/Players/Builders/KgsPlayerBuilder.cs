using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Kgs;
using OmegaGo.Core.Online.Kgs;

namespace OmegaGo.Core.Modes.LiveGame.Players.Builders
{
    public sealed class KgsPlayerBuilder : PlayerBuilder<GamePlayer, KgsPlayerBuilder>
    {
        private readonly KgsConnection _connection;

        public KgsPlayerBuilder(StoneColor color, KgsConnection connection) : base(color)
        {
            this._connection = connection;
        }

        public override GamePlayer Build()
        {
            KgsAgent igsAgent = new KgsAgent(Color, _connection);
            GamePlayer gamePlayer = new GamePlayer(CreatePlayerInfo(), igsAgent, TimeClock);
            return gamePlayer;
        }
    }
}
