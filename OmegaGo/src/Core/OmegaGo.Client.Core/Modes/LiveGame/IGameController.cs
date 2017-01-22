using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame
{
    public interface IGameController
    {
        /// <summary>
        /// Indicates that the game board has changed
        /// </summary>
        event EventHandler<GameBoard> BoardChanged;

        /// <summary>
        /// Indicates that there is a new player on turn
        /// </summary>
        event EventHandler<GamePlayer> TurnPlayerChanged;

        /// <summary>
        /// Gets the current state of the game
        /// </summary>
        GameState State { get; }

        /// <summary>
        /// Gets the pair of participating players
        /// </summary>
        PlayerPair Players { get; }

        /// <summary>
        /// Gets the player currently on turn
        /// </summary>
        GamePlayer TurnPlayer { get; }

        /// <summary>
        /// Tree of the game
        /// </summary>
        GameTree GameTree { get; }     
        
        /// <summary>
        /// Starts the game
        /// </summary>
        void BeginGame();
    }
}
