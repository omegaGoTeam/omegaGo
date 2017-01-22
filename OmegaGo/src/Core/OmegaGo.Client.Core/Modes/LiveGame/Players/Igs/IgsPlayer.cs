using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;

namespace OmegaGo.Core.Modes.LiveGame.Players.Igs
{
    public class IgsPlayer : GamePlayer
    {
        public IgsPlayer(GamePlayerType playerType, PlayerInfo playerInfo, IAgent agent) : base(playerType, playerInfo, agent)
        {
        }
    }
}
