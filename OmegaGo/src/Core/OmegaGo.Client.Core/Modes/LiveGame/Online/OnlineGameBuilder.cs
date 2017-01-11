using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class OnlineGameBuilder : GameBuilder<OnlineGame,OnlineGameBuilder>
    {
        private GamePlayer _whitePlayer = null;
        private GamePlayer _blackPlayer = null;

        public override OnlineGame Build()
        {
            throw new NotImplementedException();
        }

        public override OnlineGameBuilder SetWhitePlayer(GamePlayer player)
        {
            ValidatePlayer(player);
            _whitePlayer = player;
            return this;
        }

        public override OnlineGameBuilder SetBlackPlayer(GamePlayer player)
        {
            ValidatePlayer(player);
            _blackPlayer = player;
            return this;
        }

        protected override void ValidatePlayer(GamePlayer player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
        }
    }
}
