using OmegaGo.Core.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Agents
{
    public class AIAgent : IAgent
    {
        IAIProgram aiProgram;

        public AIAgent(IAIProgram aiProgram)
        {
            this.aiProgram = aiProgram;
        }

        public Task<AIDecision> RequestMove()
        {
            return aiProgram.RequestMove(null);
        }
    }
}
