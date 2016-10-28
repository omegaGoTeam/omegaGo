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
        Task<AIDecision> RequestMove(Game game);
    }
}
