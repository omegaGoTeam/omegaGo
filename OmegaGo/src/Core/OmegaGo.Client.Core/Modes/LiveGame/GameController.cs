using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Rules;
using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Modes.LiveGame
{
    internal class GameController : IGameController
    {
        private IGamePhase _currentGamePhase = null;
        private GameTreeNode _currentNode;
        private GamePlayer _turnPlayer;
        
        public RemoteGame OnlineGame;
        public RemoteGameInfo RemoteInfo { get; }
    
        /// <summary>
        /// Game info
        /// </summary>
        internal GameInfo Info { get; }

        public bool IsOnlineGame => Server != null;
        /// <summary>
        /// Gets the server connection, or null if this is not an online game.
        /// </summary>
        public IServerConnection Server { get; }

        public GameController(GameInfo gameInfo, IRuleset ruleset, PlayerPair players)
        {
            Debug.Assert(!(gameInfo is RemoteGameInfo));
            Info = gameInfo;
            Ruleset = ruleset;
            Players = players;
            GameTree = new GameTree(ruleset);
        }
        private GameController(RemoteGame game, IServerConnection server, IRuleset ruleset, PlayerPair players)
        {
            this.OnlineGame = game;
            RemoteInfo = game.RemoteInfo;
            Info = RemoteInfo;
            Ruleset = ruleset;
            Players = players;
            Server = server;
            GameTree = new GameTree(ruleset);

        }
        public GameController(KgsGame game, IRuleset ruleset, PlayerPair players) : this(game, game.Metadata.KgsConnection, ruleset, players)
        {
        }
        public GameController(IgsGame game, IRuleset ruleset, PlayerPair players) : this(game, game.Metadata.Server, ruleset, players)
        {
            var igsServer = game.Metadata.Server;
            igsServer.Events.TimeControlAdjustment += Events_TimeControlAdjustment;

            // Temporary: The following lines will be moved to the common constructor when life/death begins to work
            // for KGS.
            igsServer.IncomingResignation += IgsServer_IncomingResignation;
            igsServer.StoneRemoval += IgsServer_StoneRemoval; // TODO (after refactoring) < move to Life/death
            igsServer.Events.EnterLifeDeath += Events_EnterLifeDeath;
            igsServer.GameScoredAndCompleted += IgsServer_GameScoredAndCompleted;
        }

        private void Events_EnterLifeDeath(object sender, IgsGame e)
        {
            if (e.Metadata.IgsIndex == ((IgsGameInfo)this.RemoteInfo).IgsIndex)
            {
                SetPhase(GamePhaseType.LifeDeathDetermination);
            }
        }

        private void IgsServer_GameScoredAndCompleted(object sender, GameScoreEventArgs e)
        {
            // TODO this may not be our game (after refactor update)
            ((this._currentGamePhase as LifeAndDeathPhase)).ScoreIt(new Rules.Scores()
            {
                WhiteScore = e.WhiteScore,
                BlackScore = e.BlackScore
            });
        }

        private void IgsServer_StoneRemoval(object sender, StoneRemovalEventArgs e)
        {
            // TODO may not be our game
            LifeDeath_MarkGroupDead(e.DeadPosition);
        }

        private void IgsServer_IncomingResignation(object sender, GamePlayerEventArgs e)
        {
            if (this.Players.Contains(e.Player))
            {
                Resign(e.Player);
            }
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

        public event EventHandler<TerritoryMap> LifeDeathTerritoryChanged;

        /// <summary>
        /// Ruleset of the game
        /// </summary>
        public IRuleset Ruleset { get; }

        /// <summary>
        /// Players in the game
        /// </summary>
        public PlayerPair Players { get; }

        public List<Position> DeadPositions { get; set; } = new List<Position>();
        public GameEndInformation EndInformation { get; private set; }
        public void GoToEnd(GameEndInformation endInformation)
        {
            EndInformation = endInformation;
            OnGameEnded(endInformation);
            SetPhase(GamePhaseType.Finished);
        }

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
            GoToEnd(GameEndInformation.Resignation(playerToMove, this));
        }

        public event EventHandler<GameEndInformation> GameEnded;
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

        public virtual void OnLifeDeathTerritoryChanged(TerritoryMap map)
        {
            LifeDeathTerritoryChanged?.Invoke(this, map);
            BoardMustBeRefreshed?.Invoke(this, EventArgs.Empty);
        }
    }
}
