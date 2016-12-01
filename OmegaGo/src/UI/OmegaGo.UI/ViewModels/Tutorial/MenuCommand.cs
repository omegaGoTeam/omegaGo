namespace OmegaGo.UI.ViewModels.Tutorial
{
    internal class MenuCommand : ScenarioCommand
    {
        private string _firstOptionText;
        private string _secondOptionText;
        private string _firstOptionOutcome;
        private string _secondOptionOutcome;

        public override bool AllowsOptionClick => true;
        public override void OptionClick(int index, Scenario scenario)
        {
            if (index == 0)
            {
                scenario.OnSenseiMessageChanged(_firstOptionOutcome);
            }
            else
            {
                scenario.OnSenseiMessageChanged(_secondOptionOutcome);
            }
            scenario.ExecuteCommand();
        }

        public MenuCommand(ScenarioLoader.ParsedLine option1, ScenarioLoader.ParsedLine option1Then, ScenarioLoader.ParsedLine option2, ScenarioLoader.ParsedLine option2Then)
        {
            _firstOptionText = option1.FullArgument;
            _secondOptionText = option2.FullArgument;
            _firstOptionOutcome = option1Then.FullArgument;
            _secondOptionOutcome = option2Then.FullArgument;
        }

        public override LoopControl Execute(Scenario scenario)
        {
            scenario.OnSetChoices(_firstOptionText, _secondOptionText);
            return LoopControl.Stop;
        }
    }
}