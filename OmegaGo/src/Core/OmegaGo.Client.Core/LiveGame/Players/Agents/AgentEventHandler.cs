using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.LiveGame.Players.Agents;

namespace OmegaGo.Core.Modes.LiveGame.Players.Agents
{
    public delegate void AgentEventHandler(IAgent agent);

    public delegate void AgentEventHandler<in T>(IAgent agent, T eventArgs);
}
