using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Agents;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.AI
{
    public class AiPlayer : GamePlayer
    {
        public AiPlayer(GamePlayerInfo playerInfo, IObsoleteAgent agent) : base(playerInfo, agent)
        {
        }
    }
}
