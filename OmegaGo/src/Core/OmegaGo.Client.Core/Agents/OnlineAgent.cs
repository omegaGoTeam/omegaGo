using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
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
