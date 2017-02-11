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
        /// Name of the AI
        /// </summary>
        public override string Name  => "Defeatist";

        /// <summary>
        /// DefeatistAI
        /// </summary>
        public override string Description
            => @"The strongest AI program in this game, the Defeatist will resign the first time it gets the chance to. You are simply not worth its time.\n\n
                 After you make your first move, or even before that, the AI will conclude that it's much stronger than you and just resign in order to not play a game with a foregone conclusion.";

        /// <summary>
        /// Requests a move from the AI
        /// </summary>
        /// <param name="preMoveInformation">Move request info</param>
        /// <returns>AI decision</returns>
        public override AIDecision RequestMove(AIPreMoveInformation preMoveInformation)
        {
            return AIDecision.Resign("I could have won but I decided to let you win.");
        }
    }
}
