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
    internal class InGameFormGuiAgent : AgentBase
    {
        private readonly InGameForm _inGameForm;
        public readonly BufferBlock<AgentDecision> DecisionsToMake = new BufferBlock<AgentDecision>();

        public override void Click(StoneColor color, Position selectedPosition)
        {
            this._inGameForm.groupboxMoveMaker.Visible = false;
            this.Game.GameController.MakeMove(Player, Move.PlaceStone(color, selectedPosition));

        }
        public override void ForcePass(StoneColor color)
        {
            this._inGameForm.groupboxMoveMaker.Visible = false;
            this.Game.GameController.MakeMove(Player, Move.Pass(color));
        }

        public override void PleaseMakeAMove()
        {
            /* TODO
             * 
            AgentDecision storedDecision = GetStoredDecision(game);
            if (storedDecision != null)
            {
                return storedDecision;
            }
            */
            this._inGameForm.groupboxMoveMaker.Visible = true;
        }

        public InGameFormGuiAgent(InGameForm form)
        {
            this._inGameForm = form;
        }

        public override async Task<AgentDecision> RequestMoveAsync(Game game)
        {
            throw new Exception();
            //  AgentDecision decision = await this.DecisionsToMake.ReceiveAsync();
            //return decision;
        }

        public override IllegalMoveHandling HowToHandleIllegalMove => IllegalMoveHandling.Retry;
      
    }
}
