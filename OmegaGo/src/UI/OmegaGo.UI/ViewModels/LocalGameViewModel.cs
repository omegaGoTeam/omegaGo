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
using OmegaGo.Core.AI;
using System.Threading.Tasks;
using OmegaGo.UI.Localization;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace OmegaGo.UI.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LocalGameViewModel : LiveGameViewModel
    {
        private readonly Assistant _assistant;

        private bool _canUndo;
        private bool _canPass;
        private bool _outgoingUndoInProgress;

        private IMvxCommand _passCommand;
        private IMvxCommand _resignCommand;
        private IMvxCommand _undoCommand;

        private IMvxCommand _lifeAndDeathDoneCommand;
        private IMvxCommand _resumeGameCommand;
        private IMvxCommand _requestUndoDeathMarksCommand;

        // AI Assistant Help
        private IMvxCommand _getHintCommand;
        
        public LocalGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
            : base (gameSettings, questsManager, dialogService)
        {
            Game.Controller.MoveUndone += Controller_MoveUndone;
            

            // AI Assistant Service 
            _assistant = new Assistant(gameSettings, UiConnector, Game.Controller, Game.Info);
            UiConnector.AiLog += Assistant_uiConnector_AiLog;
        }


        /// <summary>
        /// Pass command from UI
        /// </summary>
        public IMvxCommand PassCommand => _passCommand ?? (_passCommand = new MvxCommand(Pass, () => CanPass));

        /// <summary>
        /// Resignation command from UI
        /// </summary>
        public IMvxCommand ResignCommand => _resignCommand ?? (_resignCommand = new MvxCommand(Resign, () => GamePhase == GamePhaseType.Main));

        /// <summary>
        /// Undo command from UI
        /// </summary>
        public IMvxCommand UndoCommand => _undoCommand ?? (_undoCommand = new MvxCommand(Undo, () => CanUndo));

        public IMvxCommand LifeAndDeathDoneCommand
            => _lifeAndDeathDoneCommand ?? (_lifeAndDeathDoneCommand = new MvxCommand(LifeAndDeathDone, () => GamePhase == GamePhaseType.LifeDeathDetermination));

        public IMvxCommand ResumeGameCommand
            => _resumeGameCommand ?? (_resumeGameCommand = new MvxCommand(ResumeGame, () => GamePhase == GamePhaseType.LifeDeathDetermination));

        public IMvxCommand RequestUndoDeathMarksCommand
            => _requestUndoDeathMarksCommand ??
            (_requestUndoDeathMarksCommand = new MvxCommand(RequestUndoDeathMarks, () => GamePhase == GamePhaseType.LifeDeathDetermination));

        public IMvxCommand GetHintCommand => _getHintCommand ??
                                             (_getHintCommand =
                                                 new MvxCommand(GetHint,
                                                     () => Assistant.ProvidesHintsFor(this.Game.Info)));


        public bool CanPass
        {
            get { return _canPass; }
            set { SetProperty(ref _canPass, value); }
        }

        public bool OutgoingUndoInProgress
        {
            get { return _outgoingUndoInProgress; }
            set { SetProperty(ref _outgoingUndoInProgress, value); }
        }

        public virtual bool ResumingGameIsPossible => true;

        public bool CanUndo
        {
            get { return _canUndo; }
            set { SetProperty(ref _canUndo, value); }
        }

        protected Assistant Assistant => _assistant;
        
        ////////////////
        // Initial setup overrides      
        ////////////////
        
        public override void Init()
        {
            Game.Controller.BeginGame();            
            UpdateTimeline();
        }

        public override void Appearing()
        {
            TabTitle = $"{Game.Info.Black.Name} vs. {Game.Info.White.Name} ({Localizer.LocalGame})";
        }

        public override async Task<bool> CanCloseViewModelAsync()
        {
            if (this.GetType() == typeof(LocalGameViewModel) && this.Game.Controller.Phase.Type != GamePhaseType.Finished)
            {
                if (await
                    DialogService.ShowConfirmationDialogAsync(Localizer.ExitLocal_Text, Localizer.ExitLocal_Caption,
                        Localizer.ExitLocal_Confirm, Localizer.Exit_ReturnToGame))
                {
                    UiConnector.AiLog -= Assistant_uiConnector_AiLog;
                    await base.CanCloseViewModelAsync();
                    Game.Controller.EndGame(GameEndInformation.CreateCancellation(Game.Controller.Players));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                UiConnector.AiLog -= Assistant_uiConnector_AiLog;
                await base.CanCloseViewModelAsync();
                return true;
            }
        }

        protected override void SetupPhaseChangeHandlers(Dictionary<GamePhaseType, Action<IGamePhase>> phaseStartHandlers, Dictionary<GamePhaseType, Action<IGamePhase>> phaseEndHandlers)
        {
            phaseStartHandlers[GamePhaseType.LifeDeathDetermination] = StartLifeAndDeathPhase;
            phaseEndHandlers[GamePhaseType.LifeDeathDetermination] = EndLifeAndDeathPhase;
        }



        ////////////////
        // State Changes      
        ////////////////

        protected override void OnBoardTapped(Position position)
        {
            // If the analyze mode is enabled handle it and return
            if (IsAnalyzeModeEnabled)
            {
                AnalyzeBoardTap(position);
                return;
            }

            // Otherwise do a normal move
            if (Game?.Controller.Phase.Type == GamePhaseType.LifeDeathDetermination)
            {
                UiConnector.RequestLifeDeathKillGroup(position);
            }
            else
            {
                UiConnector.MakeMove(position);
            }
        }

        protected override void OnGameEnded(GameEndInformation endInformation)
        {
            base.OnGameEnded(endInformation);

            GameSettings.Statistics.GameHasBeenCompleted(Game, endInformation);
            QuestsManager.GameCompleted(Game, endInformation);
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
            base.OnTurnPlayerChanged(newPlayer);

            BoardViewModel.BoardControlState.PointerOverShadowColor = newPlayer.Agent.Type == AgentType.Human ?
                newPlayer.Info.Color :
                StoneColor.None;

            // This must be updated here as well as Game.Controller.TurnPlayer is null until first OnTurnPlayerChanged. And at that time there are no more phase changes occuring.
            // CanPass and CanUndo would remained false until next phase change (life and death).
            UpdateCanPassAndUndo();
        }
        
        protected void RefreshCommands()
        {
            RaisePropertyChanged(nameof(ResumingGameIsPossible));
            ResignCommand.RaiseCanExecuteChanged();
            LifeAndDeathDoneCommand.RaiseCanExecuteChanged();
            ResumeGameCommand.RaiseCanExecuteChanged();
            RequestUndoDeathMarksCommand.RaiseCanExecuteChanged();

            UpdateCanPassAndUndo();
        }

        protected void UpdateCanPassAndUndo()
        {
            CanPass = (this.Game?.Controller?.TurnPlayer?.IsHuman ?? false) ? true : false;
            // TODO Petr this allows to undo before the beginning of the game and causes exception
            if (this.Game?.Controller?.GameTree == null)
            {
                // Game not yet initialized.
                CanUndo = false;
            }
            else if (this.Game.Controller.Players.Any(pl => pl.IsHuman))
            {
                if (Game.Controller.GameTree.LastNode.Equals(Game.Controller.GameTree.GameTreeRoot))
                {
                    CanUndo = false;
                }
                else
                {
                    // TODO Petr Please find a suitable name for this property.
                    bool value = this.Game.Controller.GameTree.PrimaryMoveTimeline.Any(
                        move =>
                        {
                            if (move.WhoMoves == StoneColor.None) return false;
                            return this.Game.Controller.Players[move.WhoMoves].IsHuman;
                        });

                    CanUndo = value;
                }
            }
            else
            {
                // No player is a local human.
                CanUndo = false;
            }

            PassCommand.RaiseCanExecuteChanged();
            UndoCommand.RaiseCanExecuteChanged();
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
            OutgoingUndoInProgress = true;
            UiConnector.RequestMainUndo();
        }
        
        private void LifeAndDeathDone()
        {
            UiConnector.RequestLifeDeathDone();
        }
        
        private void ResumeGame()
        {
            UiConnector.ForceLifeDeathReturnToMain();
        }
        
        private void RequestUndoDeathMarks()
        {
            UiConnector.RequestLifeDeathUndoDeathMarks();
        }

        private async void GetHint()
        {
            AIDecision hint =
                await
                    Assistant.Hint(this.Game.Info, this.Game.Controller.TurnPlayer, this.Game.Controller.GameTree,
                        this.Game.Controller.TurnPlayer.Info.Color);

            string content = "";
            string title = "";
            if (hint != null)
            {

                switch (hint.Kind)
                {
                    case AgentDecisionKind.Resign:
                        title = LocalizedStrings.YouShouldResign;
                        content = LocalizedStrings.ResignExplanation + " " + hint.Explanation;
                        break;
                    case AgentDecisionKind.Move:
                        title = hint.Move.ToString();
                        if (hint.Move.Kind == MoveKind.Pass)
                        {
                            content = LocalizedStrings.YouShouldPassExplanation + " " + hint.Explanation;
                        }
                        else
                        {
                            content = String.Format(LocalizedStrings.YouShouldPlayExplanation, hint.Move.Coordinates) + " " +
                                      hint.Explanation;
                        }
                        break;
                }
            }
            else
            {
                title = LocalizedStrings.HintUnavailable;
                content = LocalizedStrings.HintUnavailableExplanation;
            }
            await DialogService.ShowAsync(content, title);
            
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
            RefreshBoard(Game.Controller.GameTree.LastNode);
            // TODO Aniko, Vita: This won't work well with Analyze mode in multiplayer.
        }
        
        private void Assistant_uiConnector_AiLog(object sender, string e)
        {
            AppendLogLine($"AI: {e}");
        }
        private void Controller_MoveUndone(object sender, EventArgs e)
        {
            this.OutgoingUndoInProgress = false;
        }
    }
}
