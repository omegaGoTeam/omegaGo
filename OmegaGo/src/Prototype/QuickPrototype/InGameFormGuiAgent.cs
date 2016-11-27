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
        public readonly BufferBlock<AgentDecision> DecisionsToMake = new BufferBlock<AgentDecision>();

        public override void Click(StoneColor color, Position selectedPosition)
        {
            this.DecisionsToMake.Post(AgentDecision.MakeMove(
                OmegaGo.Core.Move.PlaceStone(color, selectedPosition), "User entered the coordinates or clicked on an intersection."));

        }
        public override void ForcePass(StoneColor color)
        {
            this.DecisionsToMake.Post(AgentDecision.MakeMove(
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
            AgentDecision decision = await this.DecisionsToMake.ReceiveAsync();
            this._inGameForm.groupboxMoveMaker.Visible = false;
            return decision;
        }

        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.Retry;
      
    }
}
