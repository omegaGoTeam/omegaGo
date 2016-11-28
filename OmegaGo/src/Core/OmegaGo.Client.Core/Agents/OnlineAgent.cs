using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
    /// <summary>
    /// Represents the agent of a player whose moves are sent to this device via an internet server, either in observation or play mode.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Agents.AgentBase" />
    public class OnlineAgent : AgentBase
    {
        public override void Click(StoneColor color, Position selectedPosition)
        {
            throw new InvalidOperationException("An online agent cannot click.");
        }

        public override void ForcePass(StoneColor color)
        {
            throw new InvalidOperationException("An online agent is not a GUI agent.");
        }

        public override void PleaseMakeAMove()
        {
            // The turn number that we're supposed to make:
            AwaitingTurnNumber = Game.NumberOfMovesPlayed;
            PossiblyAnswerAwaitingTurn();
        }

        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.PermitItAnyway;
    }
}
