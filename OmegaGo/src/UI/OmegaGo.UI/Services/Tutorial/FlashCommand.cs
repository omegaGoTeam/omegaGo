namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The "flash" command clears the game board and it's named such because it should also display a visual effect. 
    /// The visual effect was dropped due to time constraints.
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.ScenarioCommand" />
    internal class FlashCommand : ScenarioCommand
    {
        public override LoopControl Execute(Scenario scenario)
        {
            scenario.ClearBoard();
            return LoopControl.Continue;
        }
    }
}