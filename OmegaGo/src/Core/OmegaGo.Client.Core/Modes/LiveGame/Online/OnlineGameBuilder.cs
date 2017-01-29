using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Local;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class OnlineGameBuilder : GameBuilder<OnlineGame,OnlineGameBuilder>
    {
        private readonly OnlineGameInfo _info;

        public OnlineGameBuilder(OnlineGameInfo info)
        {
            this._info = info;
        }

        public override OnlineGame Build() =>
            new OnlineGame(_info, CreateRuleset(), CreatePlayers());

        protected override void ValidatePlayer(GamePlayer player)
        {
            //allows any type of player
        }
    }
}
