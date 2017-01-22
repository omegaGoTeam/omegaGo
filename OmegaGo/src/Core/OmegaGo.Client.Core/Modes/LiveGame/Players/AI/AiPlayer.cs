using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;

namespace OmegaGo.Core.Modes.LiveGame.Players.AI
{
    public class AiPlayer : GamePlayer
    {
        public AiPlayer(GamePlayerType playerType, PlayerInfo playerInfo) : base(playerType, playerInfo)
        {
        }

        public override IAgent Agent { get; }
    }
}
