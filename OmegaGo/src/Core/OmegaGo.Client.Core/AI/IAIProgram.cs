using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Represents an AI program that can intelligently play Go by making moves in response to requests by
    /// the controller application. DO NOT implement this interface directly, instead use the <see cref="AIProgramBase"/> as your base class. 
    /// </summary>
    public interface IAIProgram
    {
        /// <summary>
        /// Gets a structure that informs the core what actions, rulesets and features the AI is capable of.
        /// </summary>
        AICapabilities Capabilities { get; }

        /// <summary>
        /// Asks the AI to make a move or resign, synchronously. It is guaranteed that this method
        /// will be called in order, (TODO undos), but it may be called on white or black. The AI
        /// should check the history whether it agrees with its own. In case of a conflict,
        /// the history provided in the pre-move information takes precedence and the AI might need to erase its own history.
        /// </summary>
        /// <param name="preMoveInformation">Information the AI might need.</param>
        /// <returns>Decision</returns>
        AIDecision RequestMove (AIPreMoveInformation preMoveInformation);

        /// <summary>
        /// Asks the AI to tell us its best move for the given situation. This method must not have side-effects or
        /// be affected by the state of the class.
        /// </summary>
        /// <param name="preMoveInformation">Information the AI might need.</param>
        AIDecision GetHint(AIPreMoveInformation preMoveInformation);
    }
}
