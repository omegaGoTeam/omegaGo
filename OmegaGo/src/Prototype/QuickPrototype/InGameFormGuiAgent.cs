using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OmegaGo.Core;
using OmegaGo.Core.Agents;
using OmegaGo.Core.AI;

namespace FormsPrototype
{
    class InGameFormGuiAgent : AgentBase
    {
        private readonly InGameForm _inGameForm;
        public readonly BufferBlock<AgentDecision> _decisionsToMake = new BufferBlock<AgentDecision>();

        public override void Click(StoneColor color, Position selectedPosition)
        {
        }
        public override void ForcePass(StoneColor color)
        {
            this._decisionsToMake.Post(AgentDecision.MakeMove(
                OmegaGo.Core.Move.Pass(color), "User clicked 'PASS'."));
        }

        public InGameFormGuiAgent(InGameForm form)
        {
            this._inGameForm = form;
        }

        public override async Task<AgentDecision> RequestMoveAsync(Game game)
        {
            AgentDecision storedDecision = GetStoredDecision(game);
            if (storedDecision != null)
            {
                return storedDecision;
            }
            this._inGameForm.groupboxMoveMaker.Visible = true;
            AgentDecision decision = await this._decisionsToMake.ReceiveAsync();
            this._inGameForm.groupboxMoveMaker.Visible = false;
            return decision;
        }

        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.Retry;
      
    }
}
