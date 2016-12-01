namespace OmegaGo.UI.ViewModels.Tutorial
{
    internal class ShineCommand : ScenarioCommand
    {
        public ShineCommand(string position)
        {
            
        }

        public override LoopControl Execute(Scenario scenario)
        {
            return LoopControl.Continue;
        }
    }
}