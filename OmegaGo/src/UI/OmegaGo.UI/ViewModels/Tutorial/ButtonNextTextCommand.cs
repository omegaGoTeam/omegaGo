namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The "button [text]" command changes the Next button's caption to "[text]" until the next time the button is clicked.
    /// After that time, the button's text reverts to its standard text. This does not make the button visible.
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.ScenarioCommand" />
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