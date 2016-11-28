using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
    public class GuiAgent : AgentBase
    {
        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.Retry;
        public override void PleaseMakeAMove()
        {
            OnPleaseMakeAMove?.Invoke(this, this.Player);
        }

        public override void Click(StoneColor color, Position selectedPosition)
        {
            this.Game.GameController.MakeMove(Player, Move.PlaceStone(color, selectedPosition));

        }
        public override void ForcePass(StoneColor color)
        {
            this.Game.GameController.MakeMove(Player, Move.Pass(color));
        }

        public event EventHandler<Player> OnPleaseMakeAMove ;
    }
}
