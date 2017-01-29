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
        public override AICapabilities Capabilities => new AICapabilities(true, true, 1, int.MaxValue);

        /// <summary>
        /// Name of the AI
        /// </summary>
        public override string Name  => "Defeatist";

        public override string Description
            =>
                "The strongest AI program in this game, the DefeatistAI will resign the first time it gets the chance to. You are simply not worth its time. After you make your first move, or even before that, the AI will conclude that it's much stronger than you and just resign in order to not play a game with a foregone conclusion."
            ;

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
