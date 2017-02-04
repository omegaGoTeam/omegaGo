using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    public abstract class AiProgramBase : IAIProgram
    {
        public abstract AICapabilities Capabilities { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract AiDecision RequestMove(AIPreMoveInformation preMoveInformation);

        public virtual AiDecision GetHint(AIPreMoveInformation preMoveInformation)
        {
            if (!Capabilities.ProvidesHints)
            {
                throw new InvalidOperationException("This AI is incapable of providing hints.");
            }
            return RequestMove(preMoveInformation);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
