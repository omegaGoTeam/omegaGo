using System;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame
{
    public interface IGameController
    {
        GameState State { get; }

        GameTree GameTree { get; }

        event EventHandler BoardMustBeRefreshed;
        
        void BeginGame();

        void RespondRequest();
    }
}
