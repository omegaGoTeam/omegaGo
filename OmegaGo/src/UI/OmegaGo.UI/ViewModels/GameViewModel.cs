using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Helpers;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Connectors.UI;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using OmegaGo.Core.AI;
using OmegaGo.Core.Online.Common;
using System.Collections.ObjectModel;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.ViewModels
{
    public abstract class GameViewModel : ViewModelBase
    {
        private readonly IGameSettings _gameSettings;
        private readonly IGame _game;
        private readonly IDialogService _dialogService;
        private readonly UiConnector _uiConnector;
        private readonly IQuestsManager _questsManager;
        
        private readonly Dictionary<GamePhaseType, Action<IGamePhase>> _phaseStartHandlers;
        private readonly Dictionary<GamePhaseType, Action<IGamePhase>> _phaseEndHandlers;

        private GamePhaseType _gamePhase;

        public GameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
        {
            _gameSettings = gameSettings;
            _questsManager = questsManager;
            _dialogService = dialogService;

            _game = Mvx.GetSingleton<IGame>();
            _game.Controller.GameEnded += (s, e) => OnGameEnded(e);

            BoardViewModel = new BoardViewModel(Game.Info.BoardSize);
            BoardViewModel.BoardTapped += (s, e) => OnBoardTapped(e);

            _uiConnector = new UiConnector(Game.Controller);

            _phaseStartHandlers = new Dictionary<GamePhaseType, Action<IGamePhase>>();
            _phaseEndHandlers = new Dictionary<GamePhaseType, Action<IGamePhase>>();
            SetupPhaseChangeHandlers(_phaseStartHandlers, _phaseEndHandlers);

            Game.Controller.RegisterConnector(_uiConnector);
            Game.Controller.CurrentNodeChanged += (s, e) => OnCurrentNodeChanged(e);
            Game.Controller.CurrentNodeStateChanged += (s, e) => OnCurrentNodeStateChanged();
            Game.Controller.TurnPlayerChanged += (s, e) => OnTurnPlayerChanged(e);
            Game.Controller.GamePhaseChanged += (s, e) => OnGamePhaseChanged(e);

            ObserveDebuggingMessages();
        }
        
        public IGame Game => _game;
        public ObservableCollection<string> Log { get; } = new ObservableCollection<string>();
        
        protected IGameSettings GameSettings => _gameSettings;
        protected IDialogService DialogService => _dialogService;
        protected UiConnector UiConnector => _uiConnector;
        protected IQuestsManager QuestsManager => _questsManager;
       
        public BoardViewModel BoardViewModel
        {
            get;
            private set;
        }

        public GamePhaseType GamePhase
        {
            get { return _gamePhase; }
            set { SetProperty(ref _gamePhase, value); }
        }
        
        ////////////////
        // Initial setup overrides      
        ////////////////

        public virtual void Init()
        {
            Game.Controller.BeginGame();
        }

        protected virtual void SetupPhaseChangeHandlers(Dictionary<GamePhaseType, Action<IGamePhase>> phaseStartHandlers, Dictionary<GamePhaseType, Action<IGamePhase>> phaseEndHandlers)
        {

        }

        ////////////////
        // State Changes      
        ////////////////
      
        protected virtual void OnGameEnded(GameEndInformation endInformation)
        {

        }

        protected virtual void OnBoardTapped(Position position)
        {

        }

        protected virtual void OnCurrentNodeChanged(GameTreeNode newNode)
        {

        }

        protected virtual void OnCurrentNodeStateChanged()
        {

        }

        protected virtual void OnTurnPlayerChanged(GamePlayer newPlayer)
        {

        }
        
        protected virtual void OnGamePhaseChanged(GamePhaseChangedEventArgs phaseState)
        {
            if (phaseState.PreviousPhase != null)
            {
                _phaseEndHandlers.ItemOrDefault(phaseState.PreviousPhase.Type)?
                    .Invoke(phaseState.PreviousPhase);
            }
            
            if (phaseState.NewPhase != null)
            {
                _phaseStartHandlers.ItemOrDefault(phaseState.NewPhase.Type)?
                    .Invoke(phaseState.NewPhase);
            }

            // Define publicly the new phase
            GamePhase = phaseState.NewPhase.Type;
            
            // Should be implemented by the specific registered Action
            //if (phaseState.NewPhase.Type == GamePhaseType.LifeDeathDetermination ||
            //    phaseState.NewPhase.Type == GamePhaseType.Finished)
            //{
            //    BoardViewModel.BoardControlState.ShowTerritory = true;
            //}
            //else
            //{
            //    BoardViewModel.BoardControlState.ShowTerritory = false;
            //}
        }
        
        public virtual void Unload()
        {
            //TODO Petr : IMPLEMENT this, but using some ordinary flow like EndGame (it can be part of the IGS Game Controller logic)
            //if (this.Game is IgsGame)
            //{
            //    await ((IgsGame)this.Game).Info.Server.EndObserving((IgsGame)this.Game);
            //}
        }


        ////////////////
        // Game View Model Services      
        ////////////////
        
        protected void RefreshBoard(GameTreeNode boardState)
        {
            BoardViewModel.GameTreeNode = boardState;
            BoardViewModel.BoardControlState.TEMP_MoveLegality = Game.Controller.Ruleset.GetMoveResultLite(boardState);
            // TODO Petr: GameTree has now LastNodeChanged event - use it to fix this - for now make public and. Called from GameViewModel
            BoardViewModel.Redraw();
        }

        /// <summary>
        /// Plays a sound if its is appropriate in the given state
        /// </summary>
        /// <param name="currentState">Current game tree node</param>        
        protected async Task PlaySoundIfAppropriate(GameTreeNode state)
        {
            if (state.Branches.Count == 0)
            {
                // This is the final node.
                if (state.Move != null && state.Move.Kind != MoveKind.None)
                {
                    bool humanPlayed = (Game.Controller.Players[state.Move.WhoMoves].IsHuman);
                    bool notificationDemanded =
                        (humanPlayed
                            ? _gameSettings.Audio.PlayWhenYouPlaceStone
                            : _gameSettings.Audio.PlayWhenOthersPlaceStone) &&
                        (!(Game.Info is RemoteGameInfo) || state.MoveNumber > ((RemoteGameInfo) Game.Info).PreplayedMoveCount);
                    if (notificationDemanded)
                    {
                        if (state.Move.Kind == MoveKind.PlaceStone)
                        {
                            await Sounds.PlaceStone.PlayAsync();
                            if (state.Move.Captures.Count > 0)
                            {
                                await Sounds.Capture.PlayAsync();
                            }
                        }
                        else if (state.Move.Kind == MoveKind.Pass)
                        {
                            await Sounds.Pass.PlayAsync();
                        }
                    }
                }
            }
        }
        
        ////////////////
        // Debugging      
        ////////////////

        protected void AppendLogLine(string logLine)
        {
            Dispatcher.RequestMainThreadAction(() => Log.Add(logLine));
        }

        /// <summary>
        /// Observes debugging messages from controller
        /// </summary>
        private void ObserveDebuggingMessages()
        {
            var debuggingMessagesProvider = Game.Controller as IDebuggingMessageProvider;
            if (debuggingMessagesProvider != null)
            {
                debuggingMessagesProvider.DebuggingMessage += (s, e) => AppendLogLine(e);
            }
        }
    }
}
