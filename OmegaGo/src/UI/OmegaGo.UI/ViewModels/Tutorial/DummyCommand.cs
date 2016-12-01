namespace OmegaGo.UI.ViewModels.Tutorial
{
    internal class DummyCommand : ScenarioCommand
    {
        public override LoopControl Execute(Scenario scenario)
        {
            return LoopControl.Continue;
        }
    }
}