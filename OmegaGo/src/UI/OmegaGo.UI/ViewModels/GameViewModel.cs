﻿using MvvmCross.Platform;
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
using System.Windows.Input;
using OmegaGo.Core;
using OmegaGo.Core.Game.GameTreeConversion;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Serializing;
using OmegaGo.UI.Services.AppPackage;
using OmegaGo.UI.Services.Files;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Utility;
using OmegaGo.UI.Models.Log;

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

        private ICommand _exportSGFCommand = null;
        private ICommand _saveToLibraryCommand = null;

        public GameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
        {
            _gameSettings = gameSettings;
            _questsManager = questsManager;
            _dialogService = dialogService;

            _game = Mvx.GetSingleton<IGame>();

            BoardViewModel = new BoardViewModel(Game.Info.BoardSize);
            BoardViewModel.BoardTapped += (s, e) => OnBoardTapped(e);
            BoardViewModel.IsTouchInputOffsetEnabled = gameSettings.Display.AddTouchInputOffset;
            // Set empty node (should be in the beginning of every gametree) as current node for board rendering
            RefreshBoard(Game.Controller.GameTree.LastNode);

            _uiConnector = new UiConnector(Game.Controller);

            _phaseStartHandlers = new Dictionary<GamePhaseType, Action<IGamePhase>>();
            _phaseEndHandlers = new Dictionary<GamePhaseType, Action<IGamePhase>>();
            SetupPhaseChangeHandlers(_phaseStartHandlers, _phaseEndHandlers);

            Game.Controller.RegisterConnector(_uiConnector);

            Game.Controller.GameEnded += (s, e) => OnGameEnded(e);
            Game.Controller.GameTree.LastNodeChanged += (s, e) => OnCurrentNodeChanged(e);
            Game.Controller.TurnPlayerChanged += (s, e) => OnTurnPlayerChanged(e);
            Game.Controller.GamePhaseChanged += (s, e) => OnGamePhaseChanged(e);
            ObserveDebuggingMessages();
        }

        public IGame Game => _game;
        public ObservableCollection<LogMessage> Log { get; } = new ObservableCollection<LogMessage>();

        public ICommand ExportSGFCommand => _exportSGFCommand ??
                                               (_exportSGFCommand = new MvxAsyncCommand(ExportSGFAsync));

        public ICommand SaveToLibraryCommand => _saveToLibraryCommand ??
                                                (_saveToLibraryCommand = new MvxAsyncCommand(SaveToLibraryAsync));


        protected IGameSettings GameSettings => _gameSettings;

        protected IDialogService DialogService => _dialogService;

        protected UiConnector UiConnector => _uiConnector;

        protected IQuestsManager QuestsManager => _questsManager;

        protected virtual string SuggestedGameFileName => $"{TabTitle}_{DateTimeOffset.Now:dd-MM-yyyy}.sgf";

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
        }
        public override Task<bool> CanCloseViewModelAsync()
        {
            Game.Controller.UnsubscribeEveryoneFromController();
            return base.CanCloseViewModelAsync();
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
        }


        ////////////////
        // Game View Model Services      
        ////////////////

        protected void RefreshBoard(GameTreeNode boardState)
        {
            BoardViewModel.GameTreeNode = boardState;
            BoardViewModel.BoardControlState.TEMP_MoveLegality = Game.Controller.Ruleset.GetMoveResult(boardState);
            // TODO (future work)  Petr: GameTree has now LastNodeChanged event - use it to fix this - for now make public and. Called from GameViewModel
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
                            : _gameSettings.Audio.PlayWhenOthersPlaceStone);
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

        /// <summary>
        /// Exports SGF
        /// </summary>
        /// <returns></returns>
        private async Task ExportSGFAsync()
        {
            try
            {
                var sgf = ConvertStateToSgf();
                if (await SgfExport.ExportAsync(SuggestedGameFileName, sgf))
                {
                    Mvx.Resolve<IAppNotificationService>()
                        .TriggerNotification(new BubbleNotification(Localizer.SgfExportSuccessful, Localizer.Success,
                            NotificationType.Success));
                }
            }
            catch (Exception ex)
            {
                //ignore
            }
        }

        /// <summary>
        /// Saves game to library
        /// </summary>
        private async Task SaveToLibraryAsync()
        {
            try
            {
                var sgf = ConvertStateToSgf();
                await SgfExport.SaveToLibraryAsync(SuggestedGameFileName, sgf);
                Mvx.Resolve<IAppNotificationService>().TriggerNotification(new BubbleNotification(Localizer.SgfSaveToLibrarySuccessful, Localizer.Success, NotificationType.Success));
            }
            catch (Exception ex)
            {
                //ignore
            }
        }

        private string ConvertStateToSgf()
        {
            var appPackage = Mvx.Resolve<IAppPackageService>();
            GameTreeToSgfConverter converter = new GameTreeToSgfConverter(
                new ApplicationInfo(appPackage.AppName, appPackage.Version),
                Game.Info,
                Game.Controller.GameTree);
            var sgfGameTree = converter.Convert();
            return new SgfSerializer(true).Serialize(new SgfCollection(new[] { sgfGameTree }));
        }

        ////////////////
        // Debugging      
        ////////////////

        protected void AppendLogLine(string logLine)
        {
            Dispatcher.RequestMainThreadAction(() => Log.Add(new LogMessage(logLine)));
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
