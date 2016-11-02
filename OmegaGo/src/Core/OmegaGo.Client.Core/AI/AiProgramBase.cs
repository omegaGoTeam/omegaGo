using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    public abstract class AiProgramBase : IAIProgram
    {
        public virtual AICapabilities Capabilities { get; } = new AICapabilities(false);
        public abstract string Name { get; }
        public abstract Task<AgentDecision> RequestMove(AIPreMoveInformation gameState);

        public override string ToString()
        {
            return this.Name;
        }
    }
}
