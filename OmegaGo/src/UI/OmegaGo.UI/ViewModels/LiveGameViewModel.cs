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
using OmegaGo.UI.Services.Timer;

namespace OmegaGo.UI.ViewModels
{
    public abstract class LiveGameViewModel : GameViewModel
    {
        private int _maximumMoveIndex;
        private int _previousMoveIndex = -1;
        private int _selectedMoveIndex;
        private ITimer _portraitUpdateTimer;

        private string _instructionCaption = "";
        
        private GameEndInformation _gameEndInformation;

        public LiveGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
            : base(gameSettings, questsManager, dialogService)
        {
            BlackPortrait = new PlayerPortraitViewModel(Game.Controller.Players.Black, Game);
            WhitePortrait = new PlayerPortraitViewModel(Game.Controller.Players.White, Game);
            _portraitUpdateTimer = Mvx.Resolve<ITimerService>()
                .StartTimer(TimeSpan.FromMilliseconds(100), UpdatePortraits);
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
            _portraitUpdateTimer.End();
            return base.CanCloseViewModelAsync();
        }

        protected override async void OnGameEnded(GameEndInformation endInformation)
        {
            base.OnGameEnded(endInformation);

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
            base.OnCurrentNodeChanged(newNode);

            if (newNode != null)
            {
                UpdateTimeline();
                RefreshInstructionCaption();
                // It is ABSOLUTELY necessary for this to be the last statement in this method,
                // because we need the UpdateTimeline calls to be in order.
                await PlaySoundIfAppropriate(newNode);
                Mvx.Resolve<ITabProvider>().GetTabForViewModel(this).IsBlinking = true;
            }
        }

        protected override void OnTurnPlayerChanged(GamePlayer newPlayer)
        {
            base.OnTurnPlayerChanged(newPlayer);

            RefreshInstructionCaption();
        }

        ////////////////
        // Live Game Services      
        ////////////////

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
        // Private methods      
        ////////////////

        private void UpdatePortraits()
        {
            BlackPortrait.Update();
            WhitePortrait.Update();
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
