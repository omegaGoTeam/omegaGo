namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The "flash" command clears the game board and it should also display a visual effect.
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