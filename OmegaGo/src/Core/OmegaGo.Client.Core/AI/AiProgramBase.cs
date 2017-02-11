using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Base of AI programs
    /// </summary>
    public abstract class AIProgramBase : IAIProgram
    {
        public abstract AICapabilities Capabilities { get; }

        public abstract AIDecision RequestMove(AIPreMoveInformation preMoveInformation);

        public virtual AIDecision GetHint(AIPreMoveInformation preMoveInformation)
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
