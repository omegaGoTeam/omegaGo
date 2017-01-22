using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame
{
    /// <summary>
    /// This interface provides access to the game controller from the outside.
    /// It provides readonly state access, relevant events and methods for game manipulation.
    /// </summary>
    public interface IGameController
    {
        /// <summary>
        /// Indicates that the current game tree node has changed
        /// </summary>
        event EventHandler<GameTreeNode> CurrentGameTreeNodeChanged;

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
        /// Gets the tree of the game
        /// </summary>
        GameTree GameTree { get; }     

        /// <summary>
        /// Gets the current game tree node
        /// </summary>
        GameTreeNode CurrentGameTreeNode { get; }
        
        /// <summary>
        /// Starts the game
        /// </summary>
        void BeginGame();
    }
}
