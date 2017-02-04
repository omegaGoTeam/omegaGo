using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Modes.LiveGame
{
    internal class GameController : IGameController
    {
        private IGamePhase _currentGamePhase = null;
        private GameTreeNode _currentNode;
        private GamePlayer _turnPlayer;
        public OnlineGameInfo OnlineGameInfo;
        public OnlineGame OnlineGame;
        public bool IsOnlineGame => Server != null;
        /// <summary>
        /// Gets the server connection, or null if this is not an online game.
        /// </summary>
        public IgsConnection Server { get; }

        public GameController(GameInfo gameInfo, IRuleset ruleset, PlayerPair players)
        {
            Info = gameInfo;
            Ruleset = ruleset;
            Players = players;
            GameTree = new GameTree(ruleset);
        }
        public GameController(OnlineGame game, IRuleset ruleset, PlayerPair players)
        {
            this.OnlineGame = game;
            OnlineGameInfo = game.Metadata;
            OnlineGameInfo gameInfo = game.Metadata;
            Info = gameInfo;
            OnlineGameInfo = gameInfo;
            Ruleset = ruleset;
            Players = players;
            this.Server = gameInfo.Server;
            GameTree = new GameTree(ruleset);
            Server.Events.TimeControlAdjustment += Events_TimeControlAdjustment;
        }

        private void Events_TimeControlAdjustment(object sender, TimeControlAdjustmentEventArgs e)
        {
            if (e.Game == this.OnlineGame)
            {
                if (this.Players.Black.Clock is CanadianTimeControl)
                {
                    (this.Players.Black.Clock as CanadianTimeControl).UpdateFrom(e.Black);
                    (this.Players.White.Clock as CanadianTimeControl).UpdateFrom(e.White);
                }
            }
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
            foreach(var player in Players)
            {
                player.Agent.Resign += Agent_Resign;
            }
            //start initialization phase
            SetPhase(GamePhaseType.Initialization);
        }

        private void Agent_Resign(object sender, EventArgs e)
        {
            Resign(Players[((IAgent) sender).Color]);
        }

        public void Resign(GamePlayer playerToMove)
        {
            OnResignation(playerToMove);
            // TODO    this._game.Server?.Resign(this._game);
            SetPhase(GamePhaseType.Finished);
        }

        public event EventHandler<GamePlayer> Resignation;
        public event EventHandler<GamePlayer> PlayerTimedOut;
        private void OnResignation(GamePlayer player)
        {
            Resignation?.Invoke(this, player);
        }
        internal void OnPlayerTimedOut(GamePlayer player)
        {
            PlayerTimedOut?.Invoke(this, player);
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

        public void LifeDeath_Done(GamePlayer player)
        {
            if (Phase == GamePhaseType.LifeDeathDetermination)
            {
                (_currentGamePhase as LifeAndDeathPhase).Done(player);
            }
            else
            {
                throw new Exception("Not life/death phase.");
            }
        }

        public void LifeDeath_UndoPhase()
        {
            if (Phase == GamePhaseType.LifeDeathDetermination)
            {
                (_currentGamePhase as LifeAndDeathPhase).UndoPhase();
            }
            else
            {
                throw new Exception("Not life/death phase.");
            }
        }

        public void LifeDeath_Resume()
        {
            if (Phase == GamePhaseType.LifeDeathDetermination)
            {
                (_currentGamePhase as LifeAndDeathPhase).Resume();
            }
            else
            {
                throw new Exception("Not life/death phase.");
            }
        }

        public void LifeDeath_MarkGroupDead(Position position)
        {
            if (Phase == GamePhaseType.LifeDeathDetermination)
            {
                (_currentGamePhase as LifeAndDeathPhase).MarkGroupDead(position);
            }
            else
            {
                throw new Exception("Not life/death phase.");
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
