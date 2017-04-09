using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Connectors.UI;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Quests;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OmegaGo.UI.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LocalGameViewModel : GameViewModel
    {
        private int _maximumMoveIndex;
        private int _previousMoveIndex = -1;
        private int _selectedMoveIndex;

        private IMvxCommand _passCommand;
        private IMvxCommand _resignCommand;
        private IMvxCommand _undoCommand;

        private IMvxCommand _lifeAndDeathDoneCommand;
        private IMvxCommand _resumeGameCommand;
        private IMvxCommand _requestUndoDeathMarksCommand;
        
        public LocalGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
            : base (gameSettings, questsManager, dialogService)
        {
            BlackPortrait = new PlayerPortraitViewModel(Game.Controller.Players.Black, Game);
            WhitePortrait = new PlayerPortraitViewModel(Game.Controller.Players.White, Game);

            //TimelineViewModel = new TimelineViewModel(Game.Controller.GameTree);
            //TimelineViewModel.TimelineSelectionChanged += (s, e) => OnBoardRefreshRequested(e);
        }

        public PlayerPortraitViewModel BlackPortrait { get; }
        public PlayerPortraitViewModel WhitePortrait { get; }

        public int SelectedMoveIndex
        {
            get { return _selectedMoveIndex; }
            set
            {
                SetProperty(ref _selectedMoveIndex, value);
                GameTreeNode whatIsShowing =
                  Game.Controller.GameTree.GameTreeRoot?.GetTimelineView.Skip(value).FirstOrDefault();
                RefreshBoard(whatIsShowing);
            }
        }

        public int MaximumMoveIndex
        {
            get { return _maximumMoveIndex; }
            set { SetProperty(ref _maximumMoveIndex, value); }
        }

        /// <summary>
        /// Pass command from UI
        /// </summary>
        public IMvxCommand PassCommand => _passCommand ?? (_passCommand = new MvxCommand(Pass, () => GamePhase == GamePhaseType.Main));

        /// <summary>
        /// Resignation command from UI
        /// </summary>
        public IMvxCommand ResignCommand => _resignCommand ?? (_resignCommand = new MvxCommand(Resign, () => GamePhase == GamePhaseType.Main));

        /// <summary>
        /// Undo command from UI
        /// </summary>
        public IMvxCommand UndoCommand => _undoCommand ?? (_undoCommand = new MvxCommand(Undo, () => GamePhase == GamePhaseType.Main));

        public IMvxCommand LifeAndDeathDoneCommand
            => _lifeAndDeathDoneCommand ?? (_lifeAndDeathDoneCommand = new MvxCommand(LifeAndDeathDone, () => GamePhase == GamePhaseType.LifeDeathDetermination));

        public IMvxCommand ResumeGameCommand
            => _resumeGameCommand ?? (_resumeGameCommand = new MvxCommand(ResumeGame, () => GamePhase == GamePhaseType.LifeDeathDetermination));

        public IMvxCommand RequestUndoDeathMarksCommand
            => _requestUndoDeathMarksCommand ??
            (_requestUndoDeathMarksCommand = new MvxCommand(RequestUndoDeathMarks, () => GamePhase == GamePhaseType.LifeDeathDetermination));



        ////////////////
        // Initial setup overrides      
        ////////////////

        protected override void SetupPhaseChangeHandlers(Dictionary<GamePhaseType, Action<IGamePhase>> phaseStartHandlers, Dictionary<GamePhaseType, Action<IGamePhase>> phaseEndHandlers)
        {
            phaseStartHandlers[GamePhaseType.LifeDeathDetermination] = StartLifeAndDeathPhase;
            phaseEndHandlers[GamePhaseType.LifeDeathDetermination] = EndLifeAndDeathPhase;
        }

        public override void Init()
        {
            Game.Controller.BeginGame();
            UpdateTimeline();
        }

        ////////////////
        // State Changes      
        ////////////////

        protected override void OnBoardTapped(Position position)
        {
            if (Game?.Controller.Phase.Type == GamePhaseType.LifeDeathDetermination)
            {
                UiConnector.RequestLifeDeathKillGroup(position);
            }
            else
            {
                UiConnector.MakeMove(position);
            }
        }

        protected override async void OnGameEnded(GameEndInformation endInformation)
        {
            GameSettings.Statistics.GameHasBeenCompleted(Game, endInformation);
            QuestsManager.GameCompleted(Game, endInformation);
            await DialogService.ShowAsync(endInformation.ToString(), $"End reason: {endInformation.Reason}");
        }
        
        protected override void OnGamePhaseChanged(GamePhaseChangedEventArgs phaseState)
        {
            // Handle raising the phase handlers
            base.OnGamePhaseChanged(phaseState);

            if (phaseState.NewPhase.Type == GamePhaseType.LifeDeathDetermination ||
                phaseState.NewPhase.Type == GamePhaseType.Finished)
            {
                BoardViewModel.BoardControlState.ShowTerritory = true;
            }
            else
            {
                BoardViewModel.BoardControlState.ShowTerritory = false;
            }

            // We are in a new Game Phase, refresh commands
            RefreshCommands();
        }

        protected override void OnTurnPlayerChanged(GamePlayer newPlayer)
        {
            BoardViewModel.BoardControlState.MouseOverShadowColor = newPlayer.Agent.Type == AgentType.Human ?
                newPlayer.Info.Color :
                StoneColor.None;
        }

        protected override void OnCurrentNodeStateChanged()
        {
            RefreshBoard(Game.Controller.CurrentNode);
        }

        protected override async void OnCurrentNodeChanged(GameTreeNode newNode)
        {
            if (newNode != null)
            {
                UpdateTimeline();
                // It is ABSOLUTELY necessary for this to be the last statement in this method,
                // because we need the UpdateTimeline calls to be in order.
                await PlaySoundIfAppropriate(newNode);
            }
        }

        protected void RefreshCommands()
        {
            PassCommand.RaiseCanExecuteChanged();
            ResignCommand.RaiseCanExecuteChanged();
            UndoCommand.RaiseCanExecuteChanged();
            LifeAndDeathDoneCommand.RaiseCanExecuteChanged();
            ResumeGameCommand.RaiseCanExecuteChanged();
            RequestUndoDeathMarksCommand.RaiseCanExecuteChanged();
        }

        ////////////////
        // Mvx Commands Implementation
        ////////////////

        /// <summary>
        /// Resignation from UI
        /// </summary>
        private void Resign()
        {
            UiConnector.Resign();
        }

        /// <summary>
        /// Pass from UI
        /// </summary>
        private void Pass()
        {
            UiConnector.Pass();
        }

        /// <summary>
        /// Undo from UI
        /// </summary>
        private void Undo()
        {
            UiConnector.RequestMainUndo();
        }
        
        private void LifeAndDeathDone()
        {
            UiConnector.RequestLifeDeathDone();
        }
        
        private void ResumeGame()
        {
            Mvx.Resolve<IAppNotificationService>().TriggerNotification(new BubbleNotification("[DEBUG TEST] Resuming game."));

            UiConnector.ForceLifeDeathReturnToMain();
        }
        
        private void RequestUndoDeathMarks()
        {
            UiConnector.RequestLifeDeathUndoDeathMarks();
        }

        ////////////////
        // Timeline handling
        ////////////////

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


        ////////////////
        // Phase handlers      
        ////////////////

        private void StartLifeAndDeathPhase(IGamePhase phase)
        {
            ILifeAndDeathPhase lifeAndDeath = (ILifeAndDeathPhase)phase;
            lifeAndDeath.LifeDeathTerritoryChanged += LifeDeath_TerritoryChanged;
        }

        private void EndLifeAndDeathPhase(IGamePhase phase)
        {
            ILifeAndDeathPhase lifeAndDeath = (ILifeAndDeathPhase)phase;
            lifeAndDeath.LifeDeathTerritoryChanged -= LifeDeath_TerritoryChanged;
        }

        private void LifeDeath_TerritoryChanged(object sender, TerritoryMap e)
        {
            BoardViewModel.BoardControlState.TerritoryMap = e;
            RefreshBoard(Game.Controller.CurrentNode);
        }
    }
}
