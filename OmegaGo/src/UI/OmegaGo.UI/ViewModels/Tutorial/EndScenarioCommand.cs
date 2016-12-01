namespace OmegaGo.UI.ViewModels.Tutorial
{
    internal class EndScenarioCommand : ScenarioCommand
    {
        public override LoopControl Execute(Scenario scenario)
        {
            scenario.OnScenarioCompleted();
            return LoopControl.Stop;
        }
    }
}