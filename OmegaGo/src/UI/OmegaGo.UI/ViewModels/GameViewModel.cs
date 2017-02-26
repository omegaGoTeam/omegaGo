using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Helpers;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Connectors.UI;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Settings;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OmegaGo.UI.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GameViewModel : ViewModelBase
    {
        private readonly IGameSettings _gameSettings;
        private readonly IDialogService _dialogService;
        private readonly UiConnector _uiConnector;
        private readonly StringBuilder _systemLog = new StringBuilder();

        private ICommand _passCommand;
        private ICommand _resignCommand;
        private ICommand _undoCommand;

        private ICommand _lifeAndDeathDoneCommand;
        private ICommand _resumeGameCommand;
        private ICommand _requestUndoDeathMarksCommand;

        private string _debugInfo = "n/a";

        private int _maximumMoveIndex;

        private int _previousMoveIndex = -1;


        private int _selectedMoveIndex;


        private int frames;

        private readonly Dictionary<GamePhaseType, Action<IGamePhase>> _phaseStartHandlers =
            new Dictionary<GamePhaseType, Action<IGamePhase>>();

        private readonly Dictionary<GamePhaseType, Action<IGamePhase>> _phaseEndHandlers =
            new Dictionary<GamePhaseType, Action<IGamePhase>>();

        public GameViewModel(IGameSettings gameSettings, IDialogService dialogService)
        {
            _gameSettings = gameSettings;
            _dialogService = dialogService;
            Game = Mvx.GetSingleton<IGame>();

            _uiConnector = new UiConnector(Game.Controller);

            SetupPhaseChangeHandlers();

            Game.Controller.RegisterConnector(_uiConnector);
            Game.Controller.CurrentNodeChanged += Game_CurrentGameTreeNodeChanged;
            Game.Controller.CurrentNodeStateChanged += Game_CurrentNodeStateChanged;
            Game.Controller.TurnPlayerChanged += Controller_TurnPlayerChanged;
            Game.Controller.GamePhaseChanged += Controller_GamePhaseChanged;

            ObserveDebuggingMessages();

            Game.Controller.GameEnded += Controller_GameEnded;
            BoardViewModel = new BoardViewModel(Game.Info.BoardSize);
            BoardViewModel.BoardTapped += (s, e) => MakeMove(e);
            ChatViewModel = new ChatViewModel();
            //TODO Martin: Implement - online games will have their own ViewModel with specific properties
            //if (Game.Controller.IsOnlineGame)
            //{
            //    ChatViewModel.ChatService = new IgsChatService(Game as IgsGame);
            //    ChatViewModel.HumanAuthor = "You";
            //}
            BlackPortrait = new PlayerPortraitViewModel(Game.Controller.Players.Black);
            WhitePortrait = new PlayerPortraitViewModel(Game.Controller.Players.White);

            //TimelineViewModel = new TimelineViewModel(Game.Controller.GameTree);
            //TimelineViewModel.TimelineSelectionChanged += (s, e) => OnBoardRefreshRequested(e);
        }

        /// <summary>
        /// Observes debugging messages from controller
        /// </summary>
        [Conditional("DEBUG")]
        private void ObserveDebuggingMessages()
        {
            var debuggingMessagesProvider = Game.Controller as IDebuggingMessageProvider;
            if (debuggingMessagesProvider != null)
            {
                debuggingMessagesProvider.DebuggingMessage += (s, e) => _systemLog.AppendLine(e);
            }
        }

        /// <summary>
        /// Pass command from UI
        /// </summary>
        public ICommand PassCommand => _passCommand ?? (_passCommand = new MvxCommand(Pass));

        /// <summary>
        /// Resignation command from UI
        /// </summary>
        public ICommand ResignCommand => _resignCommand ?? (_resignCommand = new MvxCommand(Resign));

        /// <summary>
        /// Undo command from UI
        /// </summary>
        public ICommand UndoCommand => _undoCommand ?? (_undoCommand = new MvxCommand(Undo));
        
        public ICommand LifeAndDeathDoneCommand
            => _lifeAndDeathDoneCommand ?? (_lifeAndDeathDoneCommand = new MvxCommand(LifeAndDeathDone));

        public ICommand ResumeGameCommand
            => _resumeGameCommand ?? (_resumeGameCommand = new MvxCommand(ResumeGame));

        public ICommand RequestUndoDeathMarksCommand
            => _requestUndoDeathMarksCommand ??
            (_requestUndoDeathMarksCommand = new MvxCommand(RequestUndoDeathMarks));

        public string SystemLog => _systemLog.ToString();

        public IGame Game { get; }

        public BoardViewModel BoardViewModel { get; }

        public PlayerPortraitViewModel BlackPortrait { get; }
        public PlayerPortraitViewModel WhitePortrait { get; }

        public ChatViewModel ChatViewModel { get; }


        public int SelectedMoveIndex
        {
            get { return _selectedMoveIndex; }
            set
            {
                SetProperty(ref _selectedMoveIndex, value);
                GameTreeNode whatIsShowing =
                  Game.Controller.GameTree.GameTreeRoot?.GetTimelineView.Skip(value).FirstOrDefault();
                OnBoardRefreshRequested(whatIsShowing);
            }
        }

        public int MaximumMoveIndex
        {
            get { return _maximumMoveIndex; }
            set { SetProperty(ref _maximumMoveIndex, value); }
        }

        public string DebugInfo
        {
            get { return _debugInfo; }
            set { SetProperty(ref _debugInfo, value); }
        }

        private void Game_CurrentNodeStateChanged(object sender, EventArgs e)
        {
            OnBoardRefreshRequested(Game.Controller.CurrentNode);
        }

        private async void Controller_GameEnded(object sender, GameEndInformation e)
        {
            _gameSettings.Statistics.GameHasBeenCompleted(Game, e);
            _gameSettings.Quests.Events.GameCompleted(Game, e);
            await _dialogService.ShowAsync(e.ToString(), $"End reason: {e.Reason}");
        }

        private void Controller_GamePhaseChanged(object sender, GamePhaseChangedEventArgs eventArgs)
        {
            if (eventArgs.PreviousPhase != null)
            {
                _phaseEndHandlers.ItemOrDefault(eventArgs.PreviousPhase.Type)?.
                    Invoke(eventArgs.PreviousPhase);
            }

            if (eventArgs.NewPhase.Type == GamePhaseType.LifeDeathDetermination ||
                eventArgs.NewPhase.Type == GamePhaseType.Finished)
            {
                BoardViewModel.BoardControlState.ShowTerritory = true;
            }
            else
            {
                BoardViewModel.BoardControlState.ShowTerritory = false;
            }

            if (eventArgs.NewPhase != null)
            {
                _phaseStartHandlers.ItemOrDefault(eventArgs.NewPhase.Type)?.
                    Invoke(eventArgs.PreviousPhase);
            }
        }

        private void LifeDeath_TerritoryChanged(object sender, TerritoryMap e)
        {
            BoardViewModel.BoardControlState.TerritoryMap = e;
            OnBoardRefreshRequested(Game.Controller.CurrentNode);
        }

        private void Controller_TurnPlayerChanged(object sender, GamePlayer e)
        {
            BoardViewModel.BoardControlState.MouseOverShadowColor = e.Agent.Type == AgentType.Human ?
                e.Info.Color :
                StoneColor.None;
        }

        public void Init()
        {
            Game.Controller.BeginGame();
        }

        private async void Game_CurrentGameTreeNodeChanged(object sender, GameTreeNode e)
        {
            if (e != null)
            {
                UpdateTimeline();
                // It is ABSOLUTELY necessary for this to be the last statement in this method,
                // because we need the UpdateTimeline calls to be in order.
                await PlaySoundIfAppropriate(e);
            }
        }

        /// <summary>
        /// Plays a sound if its is appropriate in the current state
        /// </summary>
        /// <param name="currentState">Current game tree node</param>        
        private async Task PlaySoundIfAppropriate(GameTreeNode currentState)
        {
            if (currentState.Branches.Count == 0)
            {
                // This is the final node.
                if (currentState.Move != null)
                {
                    bool humanPlayed = (Game.Controller.Players[currentState.Move.WhoMoves].IsHuman);
                    bool notificationDemanded =
                        (humanPlayed
                            ? _gameSettings.Audio.PlayWhenYouPlaceStone
                            : _gameSettings.Audio.PlayWhenOthersPlaceStone);
                    if (notificationDemanded)
                    {
                        if (currentState.Move.Kind == MoveKind.PlaceStone)
                        {
                            await Sounds.PlaceStone.PlayAsync();
                            if (currentState.Move.Captures.Count > 0)
                            {
                                await Sounds.Capture.PlayAsync();
                            }
                        }
                        else if (currentState.Move.Kind == MoveKind.Pass)
                        {
                            await Sounds.Pass.PlayAsync();
                        }
                    }
                }
            }
        }

        public void Unload()
        {
            //TODO Petr : IMPLEMENT this, but using some ordinary flow like EndGame (it can be part of the IGS Game Controller logic)
            //if (this.Game is IgsGame)
            //{
            //    await ((IgsGame)this.Game).Info.Server.EndObserving((IgsGame)this.Game);
            //}
        }

        public void MakeMove(Position selectedPosition)
        {
            if (Game?.Controller.Phase.Type == GamePhaseType.LifeDeathDetermination)
            {
                _uiConnector.LifeDeath_RequestKillGroup(selectedPosition);
            }
            else
            {
                _uiConnector.MakeMove(selectedPosition);
            }
        }

        /// <summary>
        /// Resignation from UI
        /// </summary>
        private void Resign()
        {
            _uiConnector.Resign();
        }

        /// <summary>
        /// Pass from UI
        /// </summary>
        private void Pass()
        {
            _uiConnector.Pass();
        }

        /// <summary>
        /// Undo from UI
        /// </summary>
        private void Undo()
        {
            _uiConnector.Main_RequestUndo();
        }

        private void OnBoardRefreshRequested(GameTreeNode boardState)
        {
            BoardViewModel.GameTreeNode = boardState;
            // TODO Petr: GameTree has now LastNodeChanged event - use it to fix this - for now make public and. Called from GameViewModel
            BoardViewModel.Redraw();
        }

        private void UpdateTimeline()
        {
            var primaryTimeline = Game.Controller.GameTree.PrimaryMoveTimeline;
            int newNumber = primaryTimeline.Count() - 1;
            bool autoUpdate = newNumber == 0 || SelectedMoveIndex >= newNumber - 1;
            MaximumMoveIndex = newNumber;
            if (autoUpdate && _previousMoveIndex != newNumber)
            {
                SelectedMoveIndex = newNumber;
            }
            _previousMoveIndex = newNumber;
        }

        private void LifeAndDeathDone()
        {
            _uiConnector.LifeDeath_RequestDone();
        }
        
        private void ResumeGame()
        {
            _uiConnector.LifeDeath_ForceReturnToMain();
        }
        
        private void RequestUndoDeathMarks()
        {
            _uiConnector.LifeDeath_RequestUndoDeathMarks();
        }

        private void SetupPhaseChangeHandlers()
        {
            _phaseStartHandlers[GamePhaseType.LifeDeathDetermination] = StartLifeAndDeathPhase;
            _phaseEndHandlers[GamePhaseType.LifeDeathDetermination] = EndLifeAndDeathPhase;
        }

        private void StartLifeAndDeathPhase(IGamePhase phase)
        {
            var lifeAndDeath = (ILifeAndDeathPhase)phase;
            lifeAndDeath.LifeDeathTerritoryChanged += LifeDeath_TerritoryChanged;
        }

        private void EndLifeAndDeathPhase(IGamePhase phase)
        {
            var lifeAndDeath = (ILifeAndDeathPhase)phase;
            lifeAndDeath.LifeDeathTerritoryChanged -= LifeDeath_TerritoryChanged;
        }
    }
}
