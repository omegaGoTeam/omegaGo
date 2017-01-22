using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;

namespace OmegaGo.Core.Modes.LiveGame.Players.Local
{
    public class HumanPlayer : GamePlayer
    {
        public HumanPlayer(PlayerInfo playerInfo) : base(GamePlayerType.Human, playerInfo)
        {
            Agent = new HumanAgent(this);
        }

        /// <summary>
        /// Provides the human agent
        /// </summary>
        public override IAgent Agent { get; }
    }
}
