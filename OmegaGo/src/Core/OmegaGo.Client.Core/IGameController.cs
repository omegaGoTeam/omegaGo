using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    public interface IGameController
    {
        event EventHandler BoardMustBeRefreshed;
        event EventHandler<GameRequest> RequestRecieved;
        // TODO In future should be part of SendRequest
        void BeginGame();
        MoveResult MakeMove(Position position);
        void SendRequest(GameRequest request);
        void RespondRequest();
    }
}
