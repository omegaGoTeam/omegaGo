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
        Player TurnPlayer { get; }

        event EventHandler BoardMustBeRefreshed;
        event EventHandler<GameRequest> RequestRecieved;
        // TODO In future should be part of SendRequest
        void BeginGame();
        void SendRequest(GameRequest request);
        void RespondRequest();
    }
}
