using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
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
        /// <param name="gameInformation"></param>
        /// <returns></returns>
        public abstract AIDecision RequestMove(AiGameInformation gameInformation);

        /// <summary>
        /// Gets a hint from the AI
        /// </summary>
        /// <param name="gameInformation"></param>
        /// <returns></returns>
        public virtual AIDecision GetHint(AiGameInformation gameInformation)
        {
            if (!Capabilities.ProvidesHints)
            {
                throw new InvalidOperationException("This AI is incapable of providing hints.");
            }
            return RequestMove(gameInformation);
        }

        public virtual void MoveUndone()
        {
            // Stateless AI's don't need to do anything.
        }

        public virtual void MovePerformed(Move move, GameTree gameTree, GamePlayer informedPlayer, GameInfo info)
        {
            // Stateless AI's don't need to do anything.
        }

        public virtual Task<IEnumerable<Position>> GetDeadPositions()
        {
            throw new Exception("Nobody except Fuego supports this.");
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
