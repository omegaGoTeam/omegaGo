using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame
{
    internal class GameController : IGameController
    {
        private IGamePhase _currentGamePhase = null;
        private GameTreeNode _currentNode;
        private GamePlayer _turnPlayer;

        public GameController(GameInfo gameInfo, IRuleset ruleset, PlayerPair players)
        {
            Info = gameInfo;
            Ruleset = ruleset;
            Players = players;
            GameTree = new GameTree(ruleset);
        }

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
        /// Ruleset of the game
        /// </summary>
        public IRuleset Ruleset { get; }

        /// <summary>
        /// Players in the game
        /// </summary>
        public PlayerPair Players { get; }

        public List<Position> DeadPositions { get; set; } = new List<Position>();

        /// <summary>
        /// Game info
        /// </summary>
        internal GameInfo Info { get; }

        /// <summary>
        /// Game phase factory
        /// </summary>
        protected virtual IGameControllerPhaseFactory PhaseFactory => CreateGameControllerPhaseFactory();

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
                    _turnPlayer = value;
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
        /// Begins the game once UI is ready
        /// </summary>
        public void BeginGame()
        {
            //start initialization phase
            SetPhase(GamePhaseType.Initialization);
        }

        public void Resign(GamePlayer playerToMove)
        {
            OnResignation(playerToMove);
            // TODO    this._game.Server?.Resign(this._game);
            SetPhase(GamePhaseType.Finished);
        }

        public EventHandler<GamePlayer> Resignation;
        private void OnResignation(GamePlayer player)
        {
            Resignation?.Invoke(this, player);
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

        public event EventHandler<GamePhaseType> GamePhaseChanged;
        public void Main_Undo()
        {
            if (Phase == GamePhaseType.Main)
            {
                (_currentGamePhase as MainPhase).Undo();
            }
            else
            {
                throw new Exception("Not main phase.");
            }
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
        /// Creates the game controller phase factory based on the game info
        /// </summary>
        /// <returns>Game controller phase factory</returns>
        protected IGameControllerPhaseFactory CreateGameControllerPhaseFactory()
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

        public event EventHandler BoardMustBeRefreshed;
        public void OnBoardMustBeRefreshed()
        {
            BoardMustBeRefreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
