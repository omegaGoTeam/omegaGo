using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    public abstract class AiProgramBase : IAIProgram
    {
        public virtual AICapabilities Capabilities { get; } = new AICapabilities(false, false, 1,1);
        public abstract string Name { get; }
        public virtual string Description => "H";
        public abstract AiDecision RequestMove(AIPreMoveInformation preMoveInformation);

        public override string ToString()
        {
            return this.Name;
        }
    }
}
