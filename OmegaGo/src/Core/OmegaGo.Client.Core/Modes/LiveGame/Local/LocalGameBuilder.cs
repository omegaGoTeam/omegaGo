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
        public override LocalGame Build() => 
            new LocalGame(CreateGameInfo(), CreateRuleset(), CreatePlayers());

        protected override void ValidatePlayer(GamePlayer player)
        {
            if (player.Agent.Type != AgentType.Human &&
                player.Agent.Type != AgentType.AI)
                throw new ArgumentOutOfRangeException(nameof(player), "Local game allows human and AI players only");
        }
    }
}