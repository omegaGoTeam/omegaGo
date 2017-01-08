namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The "next" command will display the Next button and wait until the user clicks on it.
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.ScenarioCommand" />
    internal class NextCommand : ScenarioCommand
    {
        public override void ButtonClick(Scenario scenario)
        {
            scenario.ExecuteCommand();
        }
        public override LoopControl Execute(Scenario scenario)
        {
            scenario.OnNextButtonShown();
            return LoopControl.Stop;
        }
    }
}