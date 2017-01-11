using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using OmegaGo.Core.Modes.LiveGame.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Local
{
    public class LocalGameBuilder : GameBuilder<LocalGame, LocalGameBuilder>
    {
        private GamePlayer _whitePlayer = null;
        private GamePlayer _blackPlayer = null;
        private int _aiStrength = 1;

        public override LocalGame Build()
        {
            throw new NotImplementedException();
        }

        public override LocalGameBuilder SetWhitePlayer(GamePlayer player)
        {
            ValidatePlayer(player);
            _whitePlayer = player;
            return this;
        }

        public override LocalGameBuilder SetBlackPlayer(GamePlayer player)
        {
            ValidatePlayer(player);
            _blackPlayer = player;
            return this;
        }

        protected override void ValidatePlayer(GamePlayer player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));
            if (player.Info.PlayerType != Players.GamePlayerType.Human && player.Info.PlayerType != Players.GamePlayerType.AI)
                throw new ArgumentOutOfRangeException("Local game allows human and AI players only");
        }
    }
}
