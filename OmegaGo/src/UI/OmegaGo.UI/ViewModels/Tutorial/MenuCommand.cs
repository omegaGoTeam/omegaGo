namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The "menu" command must always be followed by four lines in this order:
    /// option [option 1 button caption]
    /// s [option 1 outcome text]
    /// option [option 2 button caption]
    /// s [option 2 outcome text]
    /// 
    /// This command, when executed, will display the choice dialogue to the user and then stop. When the user picks a choice,
    /// this command will change the teacher's line and then continue executing the next command (which should usually be
    /// the <see cref="NextCommand"/>). 
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.ScenarioCommand" />
    internal class MenuCommand : ScenarioCommand
    {
        private string _firstOptionText;
        private string _secondOptionText;
        private string _firstOptionOutcome;
        private string _secondOptionOutcome;
        
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