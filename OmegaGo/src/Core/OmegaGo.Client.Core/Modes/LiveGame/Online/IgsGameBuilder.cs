using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class IgsGameBuilder : GameBuilder<IgsGame,IgsGameBuilder>
    {
        private readonly IgsGameInfo _info;

        public IgsGameBuilder(IgsGameInfo info)
        {
            this._info = info;
            this.BoardSize(info.BoardSize);
            this.CountingType(Rules.CountingType.Territory);
            this.HandicapPlacementType(Rules.HandicapPlacementType.Fixed);
            this.Komi(info.Komi);
            this.WhiteHandicap(info.NumberOfHandicapStones);
        }

        public override IgsGame Build() =>
            new IgsGame(_info, CreateRuleset(), CreatePlayers());

        protected override void ValidatePlayer(GamePlayer player)
        {
            //allows any type of player
        }
    }
}
