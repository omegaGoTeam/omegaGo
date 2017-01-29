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
    public class DefeatistAI : AiProgramBase
    {
        /// <summary>
        /// Name of the AI
        /// </summary>
        public override string Name  => "DefeatistAI";

        /// <summary>
        /// Requests a move from the AI
        /// </summary>
        /// <param name="preMoveInformation">Move request info</param>
        /// <returns>AI decision</returns>
        public override AiDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            return AiDecision.Resign("I could have won but I decided to let you win.");
        }
    }
}
