using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.UI.Services.Game;
using OmegaGo.UI.Services.Tutorial;
using OmegaGo.UI.UserControls.ViewModels;
using OmegaGo.UI.ViewModels.Tutorial;
using MvvmCross.Core.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public sealed class TutorialViewModel : ViewModelBase
    {
        private string _senseisMessage;
        private string _nextButtonText;

        private string _option1Text;
        private string _option2Text;

        private bool _isChoiceDialogVisible;
        private bool _isNextButtonVisible;

        private IMvxCommand _option1Command;
        private IMvxCommand _option2Command;
        private IMvxCommand _nextButtonCommand;

        public TutorialViewModel()
        {
            BoardViewModel = new BoardViewModel(new GameBoardSize(9));
            Scenario = new BeginnerScenario();
            Scenario.GameTreeNodeChanged += Scenario_GameTreeNodeChanged;
            Scenario.ShiningPositionChanged += Scenario_ShiningPositionChanged;

            Scenario.ScenarioCompleted += Scenario_ScenarioCompleted;
            Scenario.SetChoices += Scenario_SetChoices;
            Scenario.SenseiMessageChanged += Scenario_SenseiMessageChanged;
            Scenario.NextButtonShown += Scenario_NextButtonShown;
            Scenario.NextButtonTextChanged += Scenario_NextButtonTextChanged;

            NextButtonText = Localizer.Tutorial_Next;

            Scenario.ExecuteCommand();
        }
        
        public Scenario Scenario { get; }
        public BoardViewModel BoardViewModel { get; }

        public IMvxCommand Option1Command
            => _option1Command ?? (_option1Command = new MvxCommand(Option1Selected));
        
        public IMvxCommand Option2Command
            => _option2Command ?? (_option2Command = new MvxCommand(Option2Selected));
        
        public IMvxCommand NextButtonCommand
            => _nextButtonCommand ?? (_nextButtonCommand = new MvxCommand(NextButtonPressed));
        
        public string SenseisMessage
        {
            get { return _senseisMessage; }
            set { SetProperty(ref _senseisMessage, value); }
        }

        public string NextButtonText
        {
            get { return _nextButtonText; }
            set { SetProperty(ref _nextButtonText, value); }
        }

        public string Option1Text
        {
            get { return _option1Text; }
            set { SetProperty(ref _option1Text, value); }
        }

        public string Option2Text
        {
            get { return _option2Text; }
            set { SetProperty(ref _option2Text, value); }
        }

        public bool IsChoiceDialogVisible
        {
            get { return _isChoiceDialogVisible; }
            set { SetProperty(ref _isChoiceDialogVisible, value); }
        }

        public bool IsNextButtonVisible
        {
            get { return _isNextButtonVisible; }
            set { SetProperty(ref _isNextButtonVisible, value); }
        }

        private void Scenario_ShiningPositionChanged(object sender, Position e)
        {
            BoardViewModel.BoardControlState.ShiningPosition = e;
        }

        private void Scenario_GameTreeNodeChanged(object sender, GameTreeNode e)
        {
            BoardViewModel.GameTreeNode = e;
        }

        public void TapBoardControl()
        {
            Scenario.ClickPosition(BoardViewModel.BoardControlState.PointerOverPosition);
        }
        
        // Scenario handlers

        private void Scenario_NextButtonTextChanged(object sender, string e)
        {
            NextButtonText = e;
        }

        private void Scenario_NextButtonShown(object sender, EventArgs e)
        {
            IsNextButtonVisible = true;
        }

        private void Scenario_SenseiMessageChanged(object sender, string e)
        {
            SenseisMessage = e;
        }

        private void Scenario_SetChoices(object sender, Tuple<string, string> e)
        {
            Option1Text = e.Item1;
            Option2Text = e.Item2;
            IsChoiceDialogVisible = true;
        }

        private void Scenario_ScenarioCompleted(object sender, EventArgs e)
        {
            this.GoBack();
        }

        // MvxCommand handlers

        private void Option1Selected()
        {
            IsChoiceDialogVisible = false;
            Scenario.ClickOption(0);
        }

        private void Option2Selected()
        {
            IsChoiceDialogVisible = false;
            Scenario.ClickOption(1);
        }

        private void NextButtonPressed()
        {
            IsNextButtonVisible = false;
            NextButtonText = Localizer.Tutorial_Next;
            Scenario.ClickNext();
        }
    }
}
