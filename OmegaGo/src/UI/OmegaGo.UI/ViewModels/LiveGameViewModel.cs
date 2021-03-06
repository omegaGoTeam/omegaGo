﻿using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;
using System.Linq;
using MvvmCross.Platform;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.Timer;
using OmegaGo.Core.Game.Tools;
using OmegaGo.Core.Game.Markup;
using MvvmCross.Core.ViewModels;
using System.Threading.Tasks;
using System;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.GameTools;

namespace OmegaGo.UI.ViewModels
{
    /// <summary>
    /// Represents a base class for games with competing players.
    /// </summary>
    public abstract class LiveGameViewModel : GameViewModel
    {
        // Tools
        private readonly GameToolServices _gameToolServices;
        private ITool _tool;

        // Analyze
        private bool _isAnalyzeModeEnabled;

        // System Log
        private bool _isSystemLogEnabled;

        private int _maximumMoveIndex;
        private int _selectedMoveIndex;
        private ITimer _portraitUpdateTimer;

        private string _instructionCaption = "";
        
        private GameEndInformation _gameEndInformation;

        private IMvxCommand _enableAnalyzeModeCommand;
        private IMvxCommand _disableAnalyzeModeCommand;

        public LiveGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
            : base(gameSettings, questsManager, dialogService)
        {
            BlackPortrait = new PlayerPortraitViewModel(Game.Controller.Players.Black, Game);
            WhitePortrait = new PlayerPortraitViewModel(Game.Controller.Players.White, Game);

            // Register tool services
            ToolServices = 
                new GameToolServices(
                    Localizer,
                    dialogService,
                    Game.Controller.Ruleset, 
                    Game.Controller.GameTree);
            ToolServices.PassSoundShouldBePlayed += ToolServices_PassSoundShouldBePlayed;
            ToolServices.StoneCapturesShouldBePlayed += ToolServices_StoneCapturesShouldBePlayed;
            ToolServices.StonePlacementShouldBePlayed += ToolServices_StonePlacementShouldBePlayed;
            ToolServices.NodeChanged += (s, node) => 
            {
                AnalyzeToolsViewModel.OnNodeChanged();
                RefreshBoard(node);
                GameTreeViewModel.SelectedGameTreeNode = node;
                GameTreeViewModel.RaiseGameTreeChanged();
            };
            Tool = null;

            // Initialize analyze mode and register tools
            BoardViewModel.ToolServices = ToolServices;

            AnalyzeToolsViewModel = new AnalyzeToolsViewModel(ToolServices);
            AnalyzeToolsViewModel.CanGoBackToLiveGame = true;
            RegisterAnalyzeTools();
            _isAnalyzeModeEnabled = false;

            // System log visibility
            _isSystemLogEnabled = false;

            // Set up Timeline
            GameTreeViewModel = new GameTreeViewModel(Game.Controller.GameTree);
            GameTreeViewModel.GameTreeSelectionChanged += (s, e) => 
            {
                ToolServices.Node = e;
                BoardViewModel.BoardControlState.ShowTerritory =
                    e.Equals(Game.Controller.GameTree.LastNode) &&
                    (GamePhase == GamePhaseType.LifeDeathDetermination || GamePhase == GamePhaseType.Finished);
                RefreshBoard(e);
                AnalyzeToolsViewModel.OnNodeChanged();
            };

            _portraitUpdateTimer = Mvx.Resolve<ITimerService>()
                .StartTimer(TimeSpan.FromMilliseconds(100), UpdatePortraits);

            Game.Controller.MoveUndone += (s, e) => { UpdateTimeline(); };

            // When timeline selected node changes, check whether its not in the past. If it is then disable shadow stones.
            TimelineChanged += (s, e) => BoardViewModel.BoardControlState.IsShadowDrawingEnabled = !IsTimelineInPast;
        }

        public event EventHandler TimelineChanged;

        public AnalyzeToolsViewModel AnalyzeToolsViewModel { get; }
        public GameTreeViewModel GameTreeViewModel { get; }
        public PlayerPortraitViewModel BlackPortrait { get; }
        public PlayerPortraitViewModel WhitePortrait { get; }
        
        
        protected GameToolServices ToolServices { get; }
        protected ITool Tool { get; private set; }

        public IMvxCommand EnableAnalyzeModeCommand
            => _enableAnalyzeModeCommand ?? (_enableAnalyzeModeCommand = new MvxCommand(EnableAnalyzeMode, () => !IsAnalyzeModeEnabled));

        public IMvxCommand DisableAnalyzeModeCommand
                    => _disableAnalyzeModeCommand ?? (_disableAnalyzeModeCommand = new MvxCommand(DisableAnalyzeMode, () => IsAnalyzeModeEnabled));

        /// <summary>
        /// Gets or sets a value that enables or disables analyze mode.
        /// </summary>
        public bool IsAnalyzeModeEnabled
        {
            get { return _isAnalyzeModeEnabled; }
            private set
            {
                SetProperty(ref _isAnalyzeModeEnabled, value);

                EnableAnalyzeModeCommand.RaiseCanExecuteChanged();
                DisableAnalyzeModeCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the System Log should be shown.
        /// </summary>
        public bool IsSystemLogEnabled
        {
            get { return _isSystemLogEnabled; }
            set { SetProperty(ref _isSystemLogEnabled, value); }
        }

        public bool IsTimelineInPast
        {
            get { return _selectedMoveIndex != _maximumMoveIndex; }
        }

        public int SelectedMoveIndex
        {
            get { return _selectedMoveIndex; }
            set
            {
                SetProperty(ref _selectedMoveIndex, value);
                GameTreeNode whatIsShowing = Game.Controller.GameTree.PrimaryTimelineWithRoot.Skip(value).FirstOrDefault();
                BoardViewModel.BoardControlState.ShowTerritory = 
                    (_selectedMoveIndex == _maximumMoveIndex && (GamePhase == GamePhaseType.LifeDeathDetermination || GamePhase == GamePhaseType.Finished)) ? true : false;
                RefreshBoard(whatIsShowing);

                // Notify that timeline changed - used by LocalGameView to determine whether the player Can(Pass|Undo).
                TimelineChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int MaximumMoveIndex
        {
            get { return _maximumMoveIndex; }
            set { SetProperty(ref _maximumMoveIndex, value); }
        }

        public string InstructionCaption
        {
            get { return _instructionCaption; }
            set { SetProperty(ref _instructionCaption, value); }
        }

        ////////////////
        // State Changes      
        ////////////////

        public override Task<bool> CanCloseViewModelAsync()
        {
            ToolServices.PassSoundShouldBePlayed -= ToolServices_PassSoundShouldBePlayed;
            ToolServices.StoneCapturesShouldBePlayed -= ToolServices_StoneCapturesShouldBePlayed;
            ToolServices.StonePlacementShouldBePlayed -= ToolServices_StonePlacementShouldBePlayed;
            _portraitUpdateTimer.End();
            return base.CanCloseViewModelAsync();
        }

        protected override async void OnGameEnded(GameEndInformation endInformation)
        {
            _gameEndInformation = endInformation;

            await DialogService.ShowAsync(GameEndTranslator.TranslateDetails(endInformation, Localizer),
                GameEndTranslator.TranslateCaption(endInformation, Localizer));
        }

        protected override void OnGamePhaseChanged(GamePhaseChangedEventArgs phaseState)
        {
            base.OnGamePhaseChanged(phaseState);

            RefreshInstructionCaption();
        }

        protected override async void OnCurrentNodeChanged(GameTreeNode newNode)
        {
            ITabInfo tabInfo = Mvx.Resolve<ITabProvider>().GetTabForViewModel(this);

            // This method is invoked by an event coming from Controller
            // If we are in the analyze mode, we want to change current node manually.
            if (IsAnalyzeModeEnabled)
            {
                // Notify Timeline VM that the game timeline has changed
                GameTreeViewModel.RaiseGameTreeChanged();
                await PlaySoundIfAppropriate(newNode);
                tabInfo.IsBlinking = true;
                return;
            }

            // TODO (future work)  Martin validate this hotfix
            // With handicap this method is fired much sooned and the ViewModel is not yet set, returning null.
            // Check for this case.
            if (newNode != null && tabInfo != null)
            {
                if(!IsTimelineInPast)
                    RefreshBoard(Game.Controller.GameTree.LastNode); // TODO (future work)  Vita, Aniko: This will not work well with neither timeline nor analyze mode, I think

                UpdateTimeline();
                RefreshInstructionCaption();
                // It is ABSOLUTELY necessary for this to be the last statement in this method,
                // because we need the UpdateTimeline calls to be in order.
                await PlaySoundIfAppropriate(newNode);
                tabInfo.IsBlinking = true;
            }
        }

        protected override void OnTurnPlayerChanged(GamePlayer newPlayer)
        {
            RefreshInstructionCaption();
        }

        ////////////////
        // Live Game Services      
        ////////////////
        
        protected void AnalyzeBoardTap(Position position)
        {
            // Set current pointer position
            ToolServices.PointerOverPosition = position;

            // It the current tool is not empty, execute it
            if (Tool != null)
                Tool.Execute(ToolServices);


            BoardViewModel.BoardControlState.ShowTerritory =
                ToolServices.Node.Equals(Game.Controller.GameTree.LastNode) &&
                (GamePhase == GamePhaseType.LifeDeathDetermination || GamePhase == GamePhaseType.Finished);
        }

        protected void UpdateTimeline(bool setToLast = false)
        {
            var primaryTimeline = Game.Controller.GameTree.PrimaryMoveTimelineWithRoot;
            int newMaximumTimelineIndex = primaryTimeline.Count() - 1;

            if (setToLast || SelectedMoveIndex == MaximumMoveIndex || SelectedMoveIndex >= newMaximumTimelineIndex)
            {
                // We have to keep the order as setting the selected move index fires timelinechanged event.
                // Inheriting classes then use the IsTimelineInPast, which relies on the SelectedMoveIndex and MaximumMoveIndex.
                MaximumMoveIndex = newMaximumTimelineIndex;
                SelectedMoveIndex = newMaximumTimelineIndex;
            }
            else
            {
                MaximumMoveIndex = newMaximumTimelineIndex;
            }
        }

        ////////////////
        // Mvx Commands Implementation
        ////////////////

        private void EnableAnalyzeMode()
        {
            IsAnalyzeModeEnabled = true;
            Tool = AnalyzeToolsViewModel.SelectedTool;

            BoardViewModel.Tool = Tool;
            BoardViewModel.IsShadowDrawingEnabled = true;
            BoardViewModel.IsMarkupDrawingEnabled = true;

            // Set current game node to ToolServices and Timeline VM (for node highlight)
            GameTreeNode currentNode = Game.Controller.GameTree.LastNode; // TODO (future work)  Aniko, Vita: It would be better if the current node was the node we are currently viewing, not the one that's current from the game's perspective.

            ToolServices.Node = BoardViewModel.GameTreeNode;
            GameTreeViewModel.SelectedGameTreeNode = BoardViewModel.GameTreeNode;
        }

        private void DisableAnalyzeMode()
        {
            IsAnalyzeModeEnabled = false;
            Tool = null;

            BoardViewModel.Tool = null;
            BoardViewModel.IsMarkupDrawingEnabled = false;

            RefreshBoard(Game.Controller.GameTree.LastNode);

            // Update timeline to the last position
            UpdateTimeline(true);
        }

        ////////////////
        // Private methods      
        ////////////////

        /// <summary>
        /// Registers event handlers for analyze events and registers all valid tools fot this game type.
        /// </summary>
        private void RegisterAnalyzeTools()
        {
            // Set tool when 
            AnalyzeToolsViewModel.ToolChanged += (s, tool) =>
            {
                Tool = tool;
                BoardViewModel.Tool = tool;
            };

            // When coming out of analysis, reset tool
            AnalyzeToolsViewModel.BackToGameRequested += (s, e) =>
            {
                DisableAnalyzeMode();
            };
            
            // Now register all available analysis tools for Live Games (observe, local, online)
            AnalyzeToolsViewModel.DeleteBranchTool = new DeleteBranchTool();
            AnalyzeToolsViewModel.StonePlacementTool = new StonePlacementTool(Game.Controller.GameTree.BoardSize);
            AnalyzeToolsViewModel.PassTool = new PassTool();

            AnalyzeToolsViewModel.CharacterMarkupTool = new SequenceMarkupTool(SequenceMarkupKind.Letter);
            AnalyzeToolsViewModel.NumberMarkupTool = new SequenceMarkupTool(SequenceMarkupKind.Number);
            // TODO (future work)  naming square vs rectangle o.O
            AnalyzeToolsViewModel.RectangleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Square);
            AnalyzeToolsViewModel.TriangleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Triangle);
            AnalyzeToolsViewModel.CircleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Circle);
            AnalyzeToolsViewModel.CrossMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Cross);
        }

        private void UpdatePortraits()
        {
            BlackPortrait.Update();
            WhitePortrait.Update();
        }
        private void RefreshInstructionCaption()
        {
            InstructionCaption = GenerateInstructionCaption();
        }

        private async void ToolServices_StonePlacementShouldBePlayed(object sender, EventArgs e)
        {
            await Sounds.PlaceStone.PlayAsync();
        }

        private async void ToolServices_StoneCapturesShouldBePlayed(object sender, EventArgs e)
        {
            await Sounds.Capture.PlayAsync();
        }

        private async void ToolServices_PassSoundShouldBePlayed(object sender, EventArgs e)
        {
            await Sounds.Pass.PlayAsync();
        }
        private string GenerateInstructionCaption()
        {
            // This happens during every phase change until first OnTurnPlayerChanged when TurnPlayer is filled properly.
            if (Game.Controller.TurnPlayer == null)
                return "";

            bool youAreTurnPlayer = Game.Controller.TurnPlayer.IsHuman &&
                                        !Game.Controller.Players.GetOpponentOf(Game.Controller.TurnPlayer).IsHuman;

            switch (Game.Controller.Phase.Type)
            {
                case GamePhaseType.HandicapPlacement:
                case GamePhaseType.Main:
                    string mainCaption = "";
                    if (youAreTurnPlayer)
                    {
                        mainCaption = Localizer.YourMove;
                    }
                    else if (Game.Controller.TurnPlayer.Info.Color == StoneColor.Black)
                    {
                        mainCaption = Localizer.BlackToPlay;
                    }
                    else if (Game.Controller.TurnPlayer.Info.Color == StoneColor.White)
                    {
                        mainCaption = Localizer.WhiteToPlay;
                    }
                    if (this.Game.Controller.GameTree?.LastNode?.Move?.Kind == MoveKind.Pass)
                    {
                        mainCaption += " - " + this.Localizer.OpponentPassed;
                    }
                    return mainCaption;
                case GamePhaseType.LifeDeathDetermination:
                    return Localizer.StoneRemovalPhase;
                case GamePhaseType.Finished:
                    return GameEndTranslator.TranslateCaption(_gameEndInformation, Localizer);
            }

            return "?";
        }
    }
}
