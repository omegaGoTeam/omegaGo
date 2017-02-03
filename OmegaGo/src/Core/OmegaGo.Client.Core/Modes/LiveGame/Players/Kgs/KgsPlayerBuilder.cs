using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Kgs;

namespace OmegaGo.Core.Modes.LiveGame.Players.Igs
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
