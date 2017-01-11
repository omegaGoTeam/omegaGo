using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Agents;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame.Players.Igs
{
    public class IgsPlayer : GamePlayer
    {
        public IgsPlayer(PlayerInfo playerInfo, IObsoleteAgent agent) : base(playerInfo, agent)
        {
        }
    }
}
