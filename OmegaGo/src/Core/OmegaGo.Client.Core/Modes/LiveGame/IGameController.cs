using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Agents;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Phases;

namespace OmegaGo.Core
{
    public interface IGameController
    {
        GameState State { get; }

        GameTree GameTree { get; }

        event EventHandler BoardMustBeRefreshed;
        // TODO In future should be part of SendRequest
        void BeginGame();

        void RespondRequest();
    }
}
