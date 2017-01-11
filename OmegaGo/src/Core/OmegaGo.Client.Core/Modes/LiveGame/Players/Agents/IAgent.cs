using OmegaGo.Core.Agents;
using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Agents
{
    interface IAgent
    {
        void MovePerformed(Move e, GamePlayer s);

        void MoveIllegal(MoveResult move);

        event EventHandler Move;

        IllegalMoveHandling IllegalMoveHandling { get; }

        /// <summary>
        /// Informs the agent that he is on turn
        /// </summary>
        void AgentOnTurn();
    }
}
