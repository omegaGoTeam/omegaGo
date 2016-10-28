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
        public BufferBlock<AIDecision> DecisionsToMake = new BufferBlock<AIDecision>();


        public InGameFormGuiAgent(InGameForm form)
        {
            this.inGameForm = form;
        }

        public async Task<AIDecision> RequestMove(Game game)
        {
            this.inGameForm.groupboxMoveMaker.Visible = true;
            AIDecision decision = await DecisionsToMake.ReceiveAsync();
            this.inGameForm.groupboxMoveMaker.Visible = false;
            return decision;
        }
    }
}
