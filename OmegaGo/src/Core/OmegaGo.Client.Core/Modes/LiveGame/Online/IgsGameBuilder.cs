using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class IgsGameBuilder : GameBuilder<OnlineGame,IgsGameBuilder>
    {
        private readonly OnlineGameInfo _info;

        public IgsGameBuilder(OnlineGameInfo info)
        {
            this._info = info;
            this.BoardSize(info.BoardSize);
            this.CountingType(Rules.CountingType.Territory);
            this.HandicapPlacementType(Rules.HandicapPlacementType.Fixed);
            this.Komi(info.Komi);
            this.WhiteHandicap(info.NumberOfHandicapStones);
        }

        public override OnlineGame Build() =>
            new OnlineGame(_info, CreateRuleset(), CreatePlayers());

        protected override void ValidatePlayer(GamePlayer player)
        {
            //allows any type of player
        }
    }
}
