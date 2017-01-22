using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame
{
    internal class GameController : IGameController
    {
        private IGamePhase _currentGamePhase = null;
        private GamePlayer _turnPlayer;
        private GameTreeNode _currentNode;

        public GameController(IRuleset ruleset, PlayerPair players)
        {
            Ruleset = ruleset;
            Players = players;
            GameTree = new GameTree();
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
        /// Ruleset of the game
        /// </summary>
        public IRuleset Ruleset { get; }

        /// <summary>
        /// Players in the game
        /// </summary>
        public PlayerPair Players { get; }

        /// <summary>
        /// Gets the player currently on turn
        /// </summary>
        public GamePlayer TurnPlayer
        {
            get { return _turnPlayer; }
            internal set
            {
                _turnPlayer = value;
                OnTurnPlayerChanged();
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

        protected virtual void OnTurnPlayerChanged()
        {
            TurnPlayerChanged?.Invoke(this, TurnPlayer);
        }

        protected virtual void OnCurrentGameTreeNodeChanged()
        {
            CurrentGameTreeNodeChanged?.Invoke(this, CurrentNode);
        }

        internal void SetPhase(GamePhaseType phase)
        {
            //set the new phase
            switch (phase)
            {
                case GamePhaseType.Initialization:
                    _currentGamePhase = new InitializationPhase(this);
                    break;
                case GamePhaseType.HandicapPlacement:
                    _currentGamePhase = new FinishedPhase(this);
                    break;
                case GamePhaseType.Main:
                    break;
                case GamePhaseType.LifeDeathDetermination:
                    break;
                case GamePhaseType.Finished:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }

            //inform agents about new phase and provide them access
            foreach (var player in Players)
            {
                player.Agent.GamePhaseChanged(phase);
            }

            //start phase
            _currentGamePhase.StartPhase();
        }
    }
}
