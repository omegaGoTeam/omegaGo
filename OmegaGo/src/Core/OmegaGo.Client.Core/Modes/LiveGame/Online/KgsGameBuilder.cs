using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Kgs;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class KgsGameBuilder : GameBuilder<KgsGame, KgsGameBuilder>
    {
        private readonly KgsGameInfo _info;

        public KgsGameBuilder(KgsGameInfo info)
        {
            this._info = info;
            this.BoardSize(info.BoardSize);
            this.CountingType(Rules.CountingType.Territory);
            this.HandicapPlacementType(Rules.HandicapPlacementType.Fixed);
            this.Komi(info.Komi);
            this.WhiteHandicap(info.NumberOfHandicapStones);
        }

        public override KgsGame Build() =>
            new KgsGame(_info, CreateRuleset(), CreatePlayers());

        protected override void ValidatePlayer(GamePlayer player)
        {
            //allows any type of player
        }
    }
}
