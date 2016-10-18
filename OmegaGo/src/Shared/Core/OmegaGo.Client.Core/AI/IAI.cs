using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    interface IAI
    {
        /// <summary>
        /// Gets a structure that informs the core what actions, rulesets and features the AI is capable of.
        /// </summary>
        AICapabilities Capabilities { get; }

        void SetDifficulty(int difficulty);

        Task<AIDecision> RequestMove (AIPreMoveInformation gameState);
    }
}
