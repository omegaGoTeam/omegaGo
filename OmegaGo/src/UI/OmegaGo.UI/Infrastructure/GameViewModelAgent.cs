using OmegaGo.Core;
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OmegaGo.UI.Infrastructure
{
    class GameViewModelAgent : AgentBase, IAgent
    {
        public BufferBlock<AgentDecision> DecisionsToMake = new BufferBlock<AgentDecision>();


        public GameViewModelAgent()
        {
        }

        public async Task<AgentDecision> RequestMoveAsync(Game game)
        {
            AgentDecision storedDecision = GetStoredDecision(game);
            if (storedDecision != null)
            {
                return storedDecision;
            }
            AgentDecision decision = await DecisionsToMake.ReceiveAsync();
            return decision;
        }

        public IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.Retry;
    }
}
