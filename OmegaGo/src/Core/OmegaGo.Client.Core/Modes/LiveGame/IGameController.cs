using System;
using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;
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
        event EventHandler<GameTreeNode> CurrentGameTreeNodeChanged;

        /// <summary>
        /// Debugging event
        /// </summary>
        event EventHandler<string> DebuggingMessage;

        event EventHandler<GameEndInformation> GameEnded;
        event EventHandler<TerritoryMap> LifeDeathTerritoryChanged;

        event EventHandler<GamePhaseType> GamePhaseChanged;
        event EventHandler BoardMustBeRefreshed;

        bool IsOnlineGame { get; }

        /// <summary>
        /// Gets the game's ruleset
        /// </summary>
        IRuleset Ruleset { get; }

        /// <summary>
        /// Gets the pair of participating players
        /// </summary>
        PlayerPair Players { get; }

        List<Position> DeadPositions { get; set; }
        IServerConnection Server { get; }

        /// <summary>
        /// Starts the game
        /// </summary>
        void BeginGame();

        void Resign(GamePlayer playerToMove);

        void Main_Undo();
        void LifeDeath_Done(GamePlayer player);
        void LifeDeath_UndoPhase();
        void LifeDeath_Resume();
        void LifeDeath_MarkGroupDead(Position position);

    }
}
