using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Agents;

namespace OmegaGo.Core
{
    public interface IGameController
    {
        GamePlayer TurnPlayer { get; }

        event EventHandler BoardMustBeRefreshed;
        // TODO In future should be part of SendRequest
        void BeginGame();

        void RespondRequest();
    }
}
