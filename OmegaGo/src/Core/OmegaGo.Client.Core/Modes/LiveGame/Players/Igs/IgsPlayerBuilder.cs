using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Igs;

namespace OmegaGo.Core.Modes.LiveGame.Players.Igs
{
    public sealed class IgsPlayerBuilder : PlayerBuilder<GamePlayer, IgsPlayerBuilder>
    {
        private readonly IgsConnection _pandanet;

        public IgsPlayerBuilder(StoneColor color, IgsConnection pandanet) : base(color)
        {
            this._pandanet = pandanet;
        }

        public override GamePlayer Build()
        {
            IgsAgent igsAgent = new IgsAgent(Color);
            GamePlayer gamePlayer = new GamePlayer(CreatePlayerInfo(), igsAgent, TimeClock);
            return gamePlayer;
        }
    }
}
