namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// This command is inserted at the end of the scenario and terminates the scenario, returing the player to wherever they
    /// came from.
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.ScenarioCommand" />
    internal class EndScenarioCommand : ScenarioCommand
    {
        public override LoopControl Execute(Scenario scenario)
        {
            scenario.OnScenarioCompleted();
            return LoopControl.Stop;
        }
    }
}