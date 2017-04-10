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
    public class DefeatistAI : AIProgramBase
    {
        /// <summary>
        /// Describes the AI's capabilities
        /// </summary>
        public override AICapabilities Capabilities => new AICapabilities(true, true, 1, int.MaxValue);

        /// <summary>
        /// Requests a move from the AI
        /// </summary>
        /// <param name="gameInformation">Move request info</param>
        /// <returns>AI decision</returns>
        public override AIDecision RequestMove(AiGameInformation gameInformation)
        {
            return AIDecision.Resign("I could have won but I decided to let you win.");
        }
    }
}