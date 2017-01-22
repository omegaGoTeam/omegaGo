using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame
{
    /// <summary>
    /// This interface provides access to the game controller from the outside.
    /// It provides readonly state access, relevant events and methods for game manipulation.
    /// </summary>
    public interface IGameController
    {
        /// <summary>
        /// Indicates that there is a new player on turn
        /// </summary>
        event EventHandler<GamePlayer> TurnPlayerChanged;

        /// <summary>
        /// Indicates that the current game tree node has changed
        /// </summary>
        event EventHandler<GameTreeNode> CurrentGameTreeNodeChanged;
        
        /// <summary>
        /// Gets the pair of participating players
        /// </summary>
        PlayerPair Players { get; }       

        /// <summary>
        /// Gets the tree of the game
        /// </summary>
        GameTree GameTree { get; }

        /// <summary>
        /// Gets the current game tree node
        /// </summary>
        GameTreeNode CurrentNode { get; }

        /// <summary>
        /// Gets the game's ruleset
        /// </summary>
        IRuleset Ruleset { get; }

        /// <summary>
        /// Gets the current game phase
        /// </summary>
        GamePhaseType Phase { get; }

        /// <summary>
        /// Gets the number of moves that were played
        /// </summary>
        int NumberOfMoves { get; }

        /// <summary>
        /// Starts the game
        /// </summary>
        void BeginGame();
    }
}
