using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;

namespace OmegaGo.Core.AI
{
    /// <summary>
    /// Base of AI programs
    /// </summary>
    public abstract class AIProgramBase : IAIProgram
    {
        /// <summary>
        /// Capabilities of the AI
        /// </summary>
        public abstract AICapabilities Capabilities { get; }

        /// <summary>
        /// AI agent backed by this AI
        /// </summary>
        protected AiAgent Agent { get; set; }

        /// <summary>
        /// Requests a move from the AI
        /// </summary>
        /// <param name="preMoveInformation"></param>
        /// <returns></returns>
        public abstract AIDecision RequestMove(AIPreMoveInformation preMoveInformation);

        /// <summary>
        /// Gets a hint from the AI
        /// </summary>
        /// <param name="preMoveInformation"></param>
        /// <returns></returns>
        public virtual AIDecision GetHint(AIPreMoveInformation preMoveInformation)
        {
            if (!Capabilities.ProvidesHints)
            {
                throw new InvalidOperationException("This AI is incapable of providing hints.");
            }
            return RequestMove(preMoveInformation);
        }
        
        public void SetAgent(AiAgent agent)
        {
            this.Agent = agent;
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
