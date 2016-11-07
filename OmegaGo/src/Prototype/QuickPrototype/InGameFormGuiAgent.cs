using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OmegaGo.Core;
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;

namespace QuickPrototype
{
    class InGameFormGuiAgent : IAgent
    {
        private readonly InGameForm inGameForm;
        public BufferBlock<AgentDecision> DecisionsToMake = new BufferBlock<AgentDecision>();


        public InGameFormGuiAgent(InGameForm form)
        {
            this.inGameForm = form;
        }

        public async Task<AgentDecision> RequestMove(Game game)
        {
            this.inGameForm.groupboxMoveMaker.Visible = true;
            AgentDecision decision = await DecisionsToMake.ReceiveAsync();
            this.inGameForm.groupboxMoveMaker.Visible = false;
            return decision;
        }

        public IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.Retry;
        public void ForceHistoricMove(int moveIndex, Move move)
        {
            throw new NotImplementedException();
        }
    }
}
