using OmegaGo.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame.Local
{
    public class LocalGameBuilder : GameBuilder<LocalGame, LocalGameBuilder>
    {
        private int _aiStrength = 1;

        public override LocalGame Build()
        {
            throw new NotImplementedException();
        }

        protected override void ValidatePlayer(GamePlayer player)
        {            
            if (player.Info.PlayerType != Players.GamePlayerType.Human && player.Info.PlayerType != Players.GamePlayerType.AI)
                throw new ArgumentOutOfRangeException("Local game allows human and AI players only");
        }
    }
}
