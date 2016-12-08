using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
    /// <summary>
    /// Represents the agent of a human player playing on this device. This agent works for hotseat, playing against AI and for internet play.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Agents.AgentBase" />
    public class GuiAgent : AgentBase, IReceiverOfGuiActions
    {
        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.Retry;
        public override void PleaseMakeAMove()
        {
            OnPleaseMakeAMove?.Invoke(this, Player);
        }

        public void Click(StoneColor color, Position selectedPosition)
        {
            Game.GameController.MakeMove(Player, Move.PlaceStone(color, selectedPosition));

        }
        public void ForcePass(StoneColor color)
        {
            Game.GameController.MakeMove(Player, Move.Pass(color));
        }

        /// <summary>
        /// Occurs when the game controller requests this agent's PLAYER to make a move.
        /// </summary>
        public event EventHandler<Player> OnPleaseMakeAMove ;
    }
}
