using OmegaGo.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OmegaGo.Core.Agents
{
    /*
    public class GameViewModelAgent : AgentBase, IAgent
    {
        
        private BufferBlock<AgentDecision> _decisionsToMake = new BufferBlock<AgentDecision>();

        public override async Task<AgentDecision> RequestMoveAsync(Game game)
        {
            AgentDecision storedDecision = GetStoredDecision(game);
            if (storedDecision != null)
            {
                return storedDecision;
            }
            AgentDecision decision = await this._decisionsToMake.ReceiveAsync();
            return decision;
        }

        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.Retry;

        public override void Click(StoneColor color, Position selectedPosition)
        {
            _decisionsToMake.Post(
                AgentDecision.MakeMove(Move.PlaceStone(color, selectedPosition),
                    "A click."));
        }

    }
    */
}
