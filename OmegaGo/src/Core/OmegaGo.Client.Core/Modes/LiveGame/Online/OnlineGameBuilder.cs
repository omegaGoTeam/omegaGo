using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class OnlineGameBuilder : GameBuilder<OnlineGame,OnlineGameBuilder>
    {
        public override OnlineGame Build()
        {
            throw new NotImplementedException();
        }
        
        protected override void ValidatePlayer(GamePlayer player)
        {
            //allows every type of player
        }
    }
}
