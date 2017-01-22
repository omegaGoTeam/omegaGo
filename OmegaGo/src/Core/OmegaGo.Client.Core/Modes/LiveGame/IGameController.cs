using System;
using OmegaGo.Core.Game;

namespace OmegaGo.Core.Modes.LiveGame
{
    public interface IGameController
    {
        GameState State { get; }

        PlayerPair Players { get; }

        GameTree GameTree { get; }

        event EventHandler BoardMustBeRefreshed;
        
        void BeginGame();

        void RespondRequest();
    }
}
