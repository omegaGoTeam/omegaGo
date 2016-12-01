namespace OmegaGo.UI.ViewModels.Tutorial
{
    internal class ButtonNextTextCommand : ScenarioCommand
    {
        private readonly string _newText;

        public ButtonNextTextCommand(string newText)
        {
            this._newText = newText;
        }
        public override LoopControl Execute(Scenario scenario)
        {
            scenario.OnNextButtonTextChanged(this._newText);
            return LoopControl.Continue;
        }
    }
}