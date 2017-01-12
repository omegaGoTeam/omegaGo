using System;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame
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
