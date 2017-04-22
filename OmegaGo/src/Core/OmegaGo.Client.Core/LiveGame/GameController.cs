﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Helpers;
using OmegaGo.Core.Modes.LiveGame.Connectors;
using OmegaGo.Core.Modes.LiveGame.Connectors.UI;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Fixed;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Free;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Remote;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame
{
    /// <summary>
    ///     Represents the controller of a live game.
    /// </summary>
    public class GameController : IGameController, IDebuggingMessageProvider
    {
        /// <summary>
        ///     Ordered list of phases that have been completed, chronologically. A phase is added to the list
        ///     whenever the <see cref="SetPhase(GamePhaseType)" /> method is called.
        /// </summary>
        private readonly List<IGamePhase> _previousPhases = new List<IGamePhase>();

        /// <summary>
        ///     List of connectors registered to this game controller.
        /// </summary>
        private readonly List<IGameConnector> _registeredConnectors = new List<IGameConnector>();

        /// <summary>
        ///     The current game phase.
        /// </summary>
        private IGamePhase _currentGamePhase;

        /// <summary>
        ///     The node that represents the tip of the game timeline. In other words, the current node is the last node in the
        ///     primary timeline that has not yet been undone.
        /// </summary>
        private GameTreeNode _currentNode;

        /// <summary>
        ///     The player on turn.
        /// </summary>
        private GamePlayer _turnPlayer;

        /// <summary>
        ///     Creates the game controller.
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="ruleset">Ruleset</param>
        /// <param name="players">Players</param>
        public GameController(GameInfo gameInfo, IRuleset ruleset, PlayerPair players)
        {
            if (gameInfo == null) throw new ArgumentNullException(nameof(gameInfo));
            if (ruleset == null) throw new ArgumentNullException(nameof(ruleset));
            if (players == null) throw new ArgumentNullException(nameof(players));
            this.Info = gameInfo;
            this.Ruleset = ruleset;
            this.Players = players;
            AssignPlayers();
            this.GameTree = new GameTree(ruleset, this.Info.BoardSize);
            InitGameTree();
        }

        /// <summary>
        ///     Gets a read-only list of connectors in the game.
        /// </summary>
        public IReadOnlyList<IGameConnector> Connectors =>
            new ReadOnlyCollection<IGameConnector>(this._registeredConnectors);

        /// <summary>
        ///     Get the associated metadata.
        /// </summary>
        internal GameInfo Info { get; }

        /// <summary>
        ///     Game phase factory
        /// </summary>
        protected virtual IGameControllerPhaseFactory PhaseFactory => CreateGameControllerPhaseFactory();

        /// <summary>
        ///     Occurs when a debugging message is to be printed to the user in debug mode.
        /// </summary>
        public event EventHandler<string> DebuggingMessage;

        /// <summary>
        ///     Indicates that the game has ended.
        /// </summary>
        public event EventHandler<GameEndInformation> GameEnded;

        /// <summary>
        ///     Indicates that there is a new player on turn.
        /// </summary>
        public event EventHandler<GamePlayer> TurnPlayerChanged;

        /// <summary>
        ///     Indicates that the current game tree node has changed, either because a move was undone
        ///     or because a new move was made, or because handicap stones were placed.
        /// </summary>
        public event EventHandler<GameTreeNode> CurrentNodeChanged;

        /// <summary>
        ///     Indicates that the state of the current node has changed
        ///     Imporant when the board is modified without switching node
        /// </summary>
        public event EventHandler CurrentNodeStateChanged;

        /// <summary>
        ///     Indicates that the game phase has changed.
        /// </summary>
        public event EventHandler<GamePhaseChangedEventArgs> GamePhaseChanged;

        /// <summary>
        ///     Indicates that the game phase has changed a the new phase has finished initializing. It is possible we're not in
        ///     that phase anymore,
        ///     if the phase ended before its StartPhase method ended (this happens, for example, with InitializationPhase).
        /// </summary>
        public event EventHandler<IGamePhase> GamePhaseStarted;

        /// <summary>
        ///     Gets the player currently on turn
        /// </summary>
        public GamePlayer TurnPlayer
        {
            get { return this._turnPlayer; }
            internal set
            {
                if (this._turnPlayer != value)
                {
                    this._turnPlayer?.Clock.StopClock();

                    this._turnPlayer = value;
                    this._turnPlayer.Clock.StartClock();

                    OnTurnPlayerChanged();
                }
            }
        }

        /// <summary>
        ///     Gets the current game tree node
        /// </summary>
        public GameTreeNode CurrentNode
        {
            get { return this._currentNode; }
            private set
            {
                this._currentNode = value;
                OnCurrentNodeChanged();
            }
        }

        /// <summary>
        ///     Ruleset of the game.
        /// </summary>
        public IRuleset Ruleset { get; }

        /// <summary>
        ///     Players in the game.
        /// </summary>
        public PlayerPair Players { get; }


        /// <summary>
        ///     Gets the game tree.
        /// </summary>
        public GameTree GameTree { get; }

        /// <summary>
        ///     Gets the current game phase.
        /// </summary>
        public IGamePhase Phase => this._currentGamePhase;

        /// <summary>
        ///     Gets an ordered list of phases that have been completed, chronologically. A phase is added to the list whenever the
        ///     <see cref="SetPhase(GamePhaseType)" /> method is called.
        /// </summary>
        public IEnumerable<IGamePhase> PreviousPhases => this._previousPhases;

        /// <summary>
        ///     Gets the number of moves that have already been made. If, for examples, 3 stones were placed, and now Black is on
        ///     turn, <see cref="NumberOfMoves" /> will be 3. A pass counts as a move.
        /// </summary>
        public int NumberOfMoves => this.GameTree.PrimaryTimelineLength;

        /// <summary>
        ///     Registers a connector
        /// </summary>
        /// <param name="connector">Game connector</param>
        public void RegisterConnector(IGameConnector connector)
        {
            this._registeredConnectors.Add(connector);
        }

        /// <summary>
        ///     Begins the game once UI is ready
        /// </summary>
        public void BeginGame()
        {
            SubscribePlayerEvents();
            SetPhase(GamePhaseType.Initialization);
        }

        /// <summary>
        ///     Ends the game
        /// </summary>
        /// <param name="endInformation">Game end info</param>
        public void EndGame(GameEndInformation endInformation)
        {
            OnDebuggingMessage("Game ended: " + endInformation);
            OnGameEnded(endInformation);
            SetPhase(GamePhaseType.Finished);

            UnsubscribePlayerEvents();
        }

        public event EventHandler MoveUndone;

        /// <summary>
        ///     Fires the <see cref="TurnPlayerChanged" /> event.
        /// </summary>
        protected virtual void OnTurnPlayerChanged()
        {
            TurnPlayerChanged?.Invoke(this, this.TurnPlayer);
        }

        /// <summary>
        ///     Returns a registered connector of a given type
        /// </summary>
        /// <typeparam name="T">Type of connector to return</typeparam>
        /// <returns>Connector or default in case not found</returns>
        internal T GetConnector<T>() where T : IGameConnector
        {
            return this._registeredConnectors.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        ///     Sets the default phase of the game
        /// </summary>
        /// <param name="phase">Phase type to set</param>
        internal void SetPhase(GamePhaseType phase)
        {
            //create new phase of the requested type from the factory
            var newPhase = this.PhaseFactory.CreatePhase(phase, this);
            SetPhase(newPhase);
        }

        /// <summary>
        ///     Sets a concrete phase of the game
        /// </summary>
        /// <param name="phase">Phase instance</param>
        internal void SetPhase(IGamePhase phase)
        {
            if (phase == null) throw new ArgumentNullException(nameof(phase));

            this._currentGamePhase?.EndPhase();

            var previousPhase = this._currentGamePhase;
            if (previousPhase != null)
            {
                this._previousPhases.Add(previousPhase);
            }

            OnDebuggingMessage("Now moving to " + phase.Type);

            this._currentGamePhase = phase;

            //there was no phase previously - we are just initializing
            if (previousPhase != null)
            {
                GamePhaseChanged?.Invoke(this, new GamePhaseChangedEventArgs(previousPhase, this._currentGamePhase));
            }

            //inform agents about new phase and provide them access
            foreach (var player in this.Players)
            {
                player.Agent.GamePhaseChanged(phase.Type);
            }

            //start the new phase
            this._currentGamePhase.StartPhase();

            GamePhaseStarted?.Invoke(this, this._currentGamePhase);
        }

        /// <summary>
        ///     Debugging message
        /// </summary>
        /// <param name="message"></param>
        internal void OnDebuggingMessage(string message)
        {
            DebuggingMessage?.Invoke(this, message);
        }

        /// <summary>
        ///     Switches the player on turn
        /// </summary>
        internal void SwitchTurnPlayer()
        {
            this.TurnPlayer = this.Players.GetOpponentOf(this.TurnPlayer);
        }

        /// <summary>
        ///     Fires the board refresh event
        /// </summary>
        internal void OnCurrentNodeStateChanged()
        {
            CurrentNodeStateChanged?.Invoke(this, EventArgs.Empty);
        }

        internal void OnMoveUndone()
        {
            MoveUndone?.Invoke(this, EventArgs.Empty);
        }


        protected virtual void LocalResignationHappened(GamePlayer resignor)
        {

        }
        /// <summary>
        ///     Subscribes to whole-game events raisable by agents. Whole-game events may happen regardless
        ///     of the current phase.
        /// </summary>
        private void SubscribePlayerEvents()
        {
            foreach (var player in this.Players)
            {
                player.Agent.Resigned += Agent_Resigned;
            }
        }

        /// <summary>
        ///     Unsubscribes the player events
        /// </summary>
        private void UnsubscribePlayerEvents()
        {
            foreach (var player in this.Players)
            {
                player.Agent.Resigned -= Agent_Resigned;
            }
        }

        /// <summary>
        ///     Assigns the players to this controller
        /// </summary>
        private void AssignPlayers()
        {
            foreach (var player in this.Players)
            {
                player.AssignToGame(this.Info, this);
            }
        }

        /// <summary>
        ///     Fires the game ended event
        /// </summary>
        /// <param name="endInformation"></param>
        private void OnGameEnded(GameEndInformation endInformation)
        {
            GameEnded?.Invoke(this, endInformation);
        }

        /// <summary>
        ///     Fires the current <see cref="CurrentNodeChanged" /> event.
        /// </summary>
        private void OnCurrentNodeChanged()
        {
            CurrentNodeChanged?.Invoke(this, this.CurrentNode);
        }

        /// <summary>
        ///     Handles player resignation
        /// </summary>
        /// <param name="agent">Agent that resigned</param>
        private void Agent_Resigned(IAgent agent)
        {
            //end game with resignation
            EndGame(GameEndInformation.CreateResignation(this.Players[agent.Color], this.Players));
            LocalResignationHappened(this.Players[agent.Color]);
        }

        /// <summary>
        ///     Initializes the game tree
        /// </summary>
        private void InitGameTree()
        {
            this.GameTree.LastNodeChanged += GameTree_LastNodeChanged;
        }

        /// <summary>
        ///     Handles the change of the last game tree node
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="newLastNode">New last node</param>
        private void GameTree_LastNodeChanged(object sender, GameTreeNode newLastNode)
        {
            //update the current node
            this.CurrentNode = newLastNode;
        }

        /// <summary>
        ///     Creates the game controller phase factory based on the game info
        /// </summary>
        /// <returns>Game controller phase factory</returns>
        private IGameControllerPhaseFactory CreateGameControllerPhaseFactory()
        {
            if (this.Info.HandicapPlacementType == HandicapPlacementType.Fixed)
            {
                return
                    new GenericPhaseFactory
                        <InitializationPhase, FixedHandicapPlacementPhase, LocalMainPhase, LifeAndDeathPhase,
                            FinishedPhase>();
            }
            return
                new GenericPhaseFactory
                    <InitializationPhase, FreeHandicapPlacementPhase, LocalMainPhase, LifeAndDeathPhase, FinishedPhase>();
        }
    }
}