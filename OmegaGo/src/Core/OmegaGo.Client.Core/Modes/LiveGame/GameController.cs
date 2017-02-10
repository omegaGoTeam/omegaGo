using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Helpers;
using OmegaGo.Core.Modes.LiveGame.Connectors;
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
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Events;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Modes.LiveGame
{
    public class GameController : IGameController, IDebuggingMessageProvider
    {
        private readonly List<IGameConnector> _registeredConnectors = new List<IGameConnector>();

        /// <summary>
        /// The current game phase
        /// </summary>
        private IGamePhase _currentGamePhase = null;

        /// <summary>
        /// Current game tree node
        /// </summary>
        private GameTreeNode _currentNode;

        /// <summary>
        /// Player on turn
        /// </summary>
        private GamePlayer _turnPlayer;

        /// <summary>
        /// Creates the game controller
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="ruleset">Ruleset</param>
        /// <param name="players">Players</param>
        public GameController(GameInfo gameInfo, IRuleset ruleset, PlayerPair players)
        {
            if (gameInfo == null) throw new ArgumentNullException(nameof(gameInfo));
            if (ruleset == null) throw new ArgumentNullException(nameof(ruleset));
            if (players == null) throw new ArgumentNullException(nameof(players));
            Info = gameInfo;
            Ruleset = ruleset;
            Players = players;
            AssignPlayers();
            GameTree = new GameTree(ruleset);
        }

        /// <summary>
        /// Indicates that the game has ended
        /// </summary>
        public event EventHandler<GameEndInformation> GameEnded;

        /// <summary>
        /// Indicates that there is a new player on turn
        /// </summary>
        public event EventHandler<GamePlayer> TurnPlayerChanged;

        /// <summary>
        /// Indicates that the current game tree node has changed
        /// </summary>
        public event EventHandler<GameTreeNode> CurrentGameTreeNodeChanged;

        /// <summary>
        /// Occurs when a debugging message is to be printed to the user in debug mode.
        /// </summary>
        public event EventHandler<string> DebuggingMessage;

        /// <summary>
        /// Indicates that the board must be refreshed
        /// </summary>
        public event EventHandler BoardMustBeRefreshed;

        /// <summary>
        /// Indicates that the game phase has changed
        /// </summary>
        public event EventHandler<GamePhaseType> GamePhaseChanged;

        /// <summary>
        /// Game info
        /// </summary>
        internal GameInfo Info { get; }

        /// <summary>
        /// Ruleset of the game
        /// </summary>
        public IRuleset Ruleset { get; }

        /// <summary>
        /// Players in the game
        /// </summary>
        public PlayerPair Players { get; }

        /// <summary>
        /// Connectors in the game
        /// </summary>
        public IReadOnlyList<IGameConnector> Connectors => 
            new ReadOnlyCollection<IGameConnector>(_registeredConnectors);

        /// <summary>
        /// Registers a connector
        /// </summary>
        /// <param name="connector">Game connector</param>
        public void RegisterConnector( IGameConnector connector )
        {
            _registeredConnectors.Add( connector );
        }

        /// <summary>
        /// Returns a registered connector of a given type
        /// </summary>
        /// <typeparam name="T">Type of connector to return</typeparam>
        /// <returns>Connector or default in case not found</returns>
        internal T GetConnector<T>() where T : IGameConnector
        {
            return _registeredConnectors.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Game phase factory
        /// </summary>
        protected virtual IGameControllerPhaseFactory PhaseFactory => CreateGameControllerPhaseFactory();

        /// <summary>
        /// Gets the current game tree node
        /// </summary>
        public GameTreeNode CurrentNode
        {
            get { return _currentNode; }
            internal set
            {
                _currentNode = value;
                OnCurrentGameTreeNodeChanged();
            }
        }

        /// <summary>
        /// Gets the player currently on turn
        /// </summary>
        public GamePlayer TurnPlayer
        {
            get { return _turnPlayer; }
            internal set
            {
                if (_turnPlayer != value)
                {
                    this._turnPlayer?.Clock.StopClock();

                    _turnPlayer = value;
                    this._turnPlayer.Clock.StartClock();

                    OnTurnPlayerChanged();
                }
            }
        }

        /// <summary>
        /// Gets the current game phase
        /// </summary>
        public GamePhaseType Phase => _currentGamePhase.PhaseType;

        /// <summary>
        /// Gets the current number of moves
        /// </summary>
        public int NumberOfMoves { get; internal set; }

        /// <summary>
        /// Gets the game tree
        /// </summary>
        public GameTree GameTree { get; }


        public void EndGame(GameEndInformation endInformation)
        {
            OnGameEnded(endInformation);
            SetPhase(GamePhaseType.Finished);
        }

        /// <summary>
        /// Begins the game once UI is ready
        /// </summary>
        public void BeginGame()
        {
            foreach (var player in Players)
            {
                player.Agent.Resigned += AgentResigned;
            }
            //start initialization phase
            SetPhase(GamePhaseType.Initialization);
        }

        private void AgentResigned(IAgent sender)
        {
            Resign(Players[((IAgent)sender).Color]);
        }

        public void Resign(GamePlayer playerToMove)
        {
            EndGame(GameEndInformation.CreateResignation(playerToMove, Players));
        }


        /// <summary>
        /// Assigns the players to this controller
        /// </summary>
        private void AssignPlayers()
        {
            foreach (var player in Players)
            {
                player.AssignToGame(Info, this);
            }
        }

        private void OnGameEnded(GameEndInformation endInformation)
        {
            GameEnded?.Invoke(this, endInformation);
        }

        protected virtual void OnTurnPlayerChanged()
        {
            TurnPlayerChanged?.Invoke(this, TurnPlayer);
            //notify the agent about his turn       
            TurnPlayer?.Agent.OnTurn();
        }

        protected virtual void OnCurrentGameTreeNodeChanged()
        {
            CurrentGameTreeNodeChanged?.Invoke(this, CurrentNode);
        }
        

        internal void SetPhase(GamePhaseType phase)
        {
            this._currentGamePhase?.EndPhase();
            OnDebuggingMessage("Now moving to " + phase);
            //set the new phase
            var newPhase = PhaseFactory.CreatePhase(phase, this);
            _currentGamePhase = newPhase;

            GamePhaseChanged?.Invoke(this, phase);

            //inform agents about new phase and provide them access
            foreach (var player in Players)
            {
                player.Agent.GamePhaseChanged(phase);
            }

            //start the new phase
            _currentGamePhase.StartPhase();
        }

        internal void OnDebuggingMessage(string message)
        {
            DebuggingMessage?.Invoke(this, message);
        }

        /// <summary>
        /// Switches the player on turn
        /// </summary>
        internal void SwitchTurnPlayer()
        {
            TurnPlayer = Players.GetOpponentOf(TurnPlayer);
        }

        /// <summary>
        /// Fires the board refresh event
        /// </summary>
        internal void OnBoardMustBeRefreshed()
        {
            BoardMustBeRefreshed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Creates the game controller phase factory based on the game info
        /// </summary>
        /// <returns>Game controller phase factory</returns>
        private IGameControllerPhaseFactory CreateGameControllerPhaseFactory()
        {
            if (Info.HandicapPlacementType == HandicapPlacementType.Fixed)
            {
                return
                    new GenericPhaseFactory
                        <InitializationPhase, FixedHandicapPlacementPhase, MainPhase, LifeAndDeathPhase, FinishedPhase>();
            }
            else
            {
                return
                    new GenericPhaseFactory
                        <InitializationPhase, FreeHandicapPlacementPhase, MainPhase, LifeAndDeathPhase, FinishedPhase>();
            }
        }
    }
}
