using MvvmCross.Core.ViewModels;
using OmegaGo.UI.Services.Dialogs;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;
using System;

namespace OmegaGo.UI.ViewModels
{
    public abstract class AnalysisGameViewModel : GameViewModel
    {
        private bool _isInAnalysisMode;

        private MvxCommand _enableAnalysisModeCommand;
        private MvxCommand _disableAnalysisModeCommand;

        public AnalysisViewModel AnalysisViewModel { get; private set; }
        public object AnalysisTool => AnalysisViewModel.SelectedTool;

        public bool IsInAnalysisMode
        {
            get { return _isInAnalysisMode; }
            set { SetProperty(ref _isInAnalysisMode, value); }
        }

        public MvxCommand EnableAnalysisModeCommand => _enableAnalysisModeCommand ?? (_enableAnalysisModeCommand = new MvxCommand(
            () => { EnableAnalysisMode(); },
            () => !IsInAnalysisMode));

        public MvxCommand DisableAnalysisModeCommand => _disableAnalysisModeCommand ?? (_disableAnalysisModeCommand = new MvxCommand(
            () => { DisableAnalysisMode(); },
            () => IsInAnalysisMode));

        public AnalysisGameViewModel(IGameSettings gameSettings, IQuestsManager questsManager, IDialogService dialogService)
            : base(gameSettings, questsManager, dialogService)
        {
            AnalysisViewModel = new AnalysisViewModel();
            AnalysisViewModel.BackToGameRequested += (s, e) => OnAnalysisBackToGameRequested();
            AnalysisViewModel.BranchDeletionRequested += (s, e) => OnAnalysisBranchDeletionRequested();
            AnalysisViewModel.PassRequested += (s, e) => OnAnalysisPassRequested();
        }

        protected virtual void OnAnalysisModeEnabled()
        {

        }

        protected virtual void OnAnalysisModeDisabled()
        {

        }

        protected virtual void OnAnalysisBackToGameRequested()
        {
            DisableAnalysisMode();
        }

        protected virtual void OnAnalysisBranchDeletionRequested()
        {

        }

        protected virtual void OnAnalysisPassRequested()
        {

        }

        protected void EnableAnalysisMode()
        {
            this.BoardViewModel.IsMarkupDrawingEnabled = true;

            IsInAnalysisMode = true;
            OnAnalysisModeEnabled();
            EnableAnalysisModeCommand.RaiseCanExecuteChanged();
            DisableAnalysisModeCommand.RaiseCanExecuteChanged();
        }

        protected void DisableAnalysisMode()
        {
            this.BoardViewModel.IsMarkupDrawingEnabled = false;

            IsInAnalysisMode = false;
            OnAnalysisModeDisabled();
            EnableAnalysisModeCommand.RaiseCanExecuteChanged();
            DisableAnalysisModeCommand.RaiseCanExecuteChanged();
        }
    }
}
