using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.Core.Game.Tools;
using OmegaGo.Core.Game.Markup;
using MvvmCross.Core.ViewModels;

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
        private int _previousMoveIndex = -1;
        private int _selectedMoveIndex;

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
                    Game.Controller.Ruleset, 
                    Game.Controller.GameTree);
            ToolServices.NodeChanged += (s, node) => 
            {
                AnalyzeViewModel.OnNodeChanged();
                RefreshBoard(node);
                TimelineViewModel.SelectedTimelineNode = node;
                TimelineViewModel.RaiseGameTreeChanged();
            };
            Tool = null;

            // Initialize analyze mode and register tools
            AnalyzeViewModel = new AnalyzeViewModel(ToolServices);
            RegisterAnalyzeTools();
            _isAnalyzeModeEnabled = false;

            // System log visibility
            _isSystemLogEnabled = false;

            // Set up Timeline
            TimelineViewModel = new TimelineViewModel(Game.Controller.GameTree);
            TimelineViewModel.TimelineSelectionChanged += (s, e) => { ToolServices.Node = e; RefreshBoard(e); };
        }

        public AnalyzeViewModel AnalyzeViewModel { get; }
        public TimelineViewModel TimelineViewModel { get; }
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

        public string InstructionCaption
        {
            get { return _instructionCaption; }
            set { SetProperty(ref _instructionCaption, value); }
        }

        ////////////////
        // State Changes      
        ////////////////
        
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
                TimelineViewModel.RaiseGameTreeChanged();
                return;
            }
            
            // TODO Martin validate this hotfix
            // With handicap this method is fired much sooned and the ViewModel is not yet set, returning null.
            // Check for this case.
            if (newNode != null && tabInfo != null)
            {
                RefreshBoard(Game.Controller.CurrentNode);
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
        }
        
        protected void UpdateTimeline()
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
        // Mvx Commands Implementation
        ////////////////

        private void EnableAnalyzeMode()
        {
            IsAnalyzeModeEnabled = true;
            Tool = AnalyzeViewModel.SelectedTool;
            BoardViewModel.Tool = Tool as IMarkupTool;

            // Set current game node to ToolServices and Timeline VM (for node highlight)
            GameTreeNode currentNode = Game.Controller.CurrentNode;

            ToolServices.Node = currentNode;
            TimelineViewModel.SelectedTimelineNode = currentNode;
        }

        private void DisableAnalyzeMode()
        {
            IsAnalyzeModeEnabled = false;
            Tool = null;
            BoardViewModel.Tool = null;

            RefreshBoard(Game.Controller.CurrentNode);
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
            AnalyzeViewModel.ToolChanged += (s, tool) =>
            {
                Tool = tool;
                BoardViewModel.Tool = tool as IMarkupTool;
            };

            // When coming out of analysis, reset tool
            AnalyzeViewModel.BackToGameRequested += (s, e) =>
            {
                DisableAnalyzeMode();
            };
            
            // Now register all available analysis tools for Live Games (observe, local, online)
            AnalyzeViewModel.DeleteBranchTool = new DeleteBranchTool();
            AnalyzeViewModel.StonePlacementTool = new StonePlacementTool(Game.Controller.GameTree.BoardSize);
            AnalyzeViewModel.PassTool = new PassTool();

            AnalyzeViewModel.CharacterMarkupTool = new SequenceMarkupTool(SequenceMarkupKind.Letter);
            AnalyzeViewModel.NumberMarkupTool = new SequenceMarkupTool(SequenceMarkupKind.Number);
            // TODO naming square vs rectangle o.O
            AnalyzeViewModel.RectangleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Square);
            AnalyzeViewModel.TriangleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Triangle);
            AnalyzeViewModel.CircleMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Circle);
            AnalyzeViewModel.CrossMarkupTool = new SimpleMarkupTool(SimpleMarkupKind.Cross);
        }

        private void RefreshInstructionCaption()
        {
            InstructionCaption = GenerateInstructionCaption();
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
