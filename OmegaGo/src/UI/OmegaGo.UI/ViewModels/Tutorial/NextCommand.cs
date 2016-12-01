namespace OmegaGo.UI.ViewModels.Tutorial
{
    internal class NextCommand : ScenarioCommand
    {
        public override bool AllowsButtonClick => true;
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