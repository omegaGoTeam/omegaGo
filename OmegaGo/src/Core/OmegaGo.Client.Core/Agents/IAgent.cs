using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI;

namespace OmegaGo.Core.Agents
{
    public interface IAgent
    {
        Task<AgentDecision> RequestMove(Game game);
        IllegalMoveHandling HowToHandleIllegalMove { get; }
        void ForceHistoricMove(int moveIndex, Move move);
    }
}
