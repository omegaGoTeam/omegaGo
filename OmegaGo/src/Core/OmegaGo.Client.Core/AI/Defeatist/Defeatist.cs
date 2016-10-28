using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Defeatist
{
    /// <summary>
    /// This super strong AI will resign the first time it gets the chance to.
    /// </summary>
    public class Defeatist : IAIProgram
    {
        public AICapabilities Capabilities => new AICapabilities();

        public Task<AIDecision> RequestMove(AIPreMoveInformation gameState)
        {
            return Task.FromResult(AIDecision.Resign("I could have won but I decided to let you win."));
        }
    }
}
