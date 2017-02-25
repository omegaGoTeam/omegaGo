using System;
using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Connectors;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame
{
    /// <summary>
    /// This interface provides access to the game controller from the outside.
    /// It provides readonly state access, relevant events and methods for game manipulation.
    /// </summary>
    public interface IGameController : IGameState
    {
        /// <summary>
        /// Indicates that there is a new player on turn
        /// </summary>
        event EventHandler<GamePlayer> TurnPlayerChanged;

        /// <summary>
        /// Indicates that the current game tree node has changed
        /// </summary>
        event EventHandler<GameTreeNode> CurrentNodeChanged;

        /// <summary>
        /// Indicates taht the current game has ended
        /// </summary>
        event EventHandler<GameEndInformation> GameEnded;

        /// <summary>
        /// Indicates that the game phase has changed
        /// </summary>
        event EventHandler<GamePhaseChangedEventArgs> GamePhaseChanged;

        /// <summary>
        /// Indicates that the game board must be refreshed
        /// </summary>
        event EventHandler CurrentNodeStateChanged;

        /// <summary>
        /// Gets the game's ruleset
        /// </summary>
        IRuleset Ruleset { get; }

        /// <summary>
        /// Gets the pair of participating players
        /// </summary>
        PlayerPair Players { get; }
        
        /// <summary>
        /// Starts the game
        /// </summary>
        void BeginGame();

        /// <summary>
        /// Ends the game
        /// </summary>
        /// <param name="endInformation">Game end info</param>
        void EndGame(GameEndInformation endInformation);

        /// <summary>
        /// Registers a connector
        /// </summary>
        /// <param name="connector">Connector to register</param>
        void RegisterConnector(IGameConnector connector);
    }
}
