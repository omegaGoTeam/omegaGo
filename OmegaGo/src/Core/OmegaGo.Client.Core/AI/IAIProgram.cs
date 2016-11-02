using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Represents an AI program that can intelligently play Go by making moves in response to requests by
    /// the controller application.
    /// </summary>
    public interface IAIProgram
    {
        /// <summary>
        /// Gets a structure that informs the core what actions, rulesets and features the AI is capable of.
        /// </summary>
        AICapabilities Capabilities { get; }
        string Name { get; }
        Task<AgentDecision> RequestMove (AIPreMoveInformation gameState);
    }
}
