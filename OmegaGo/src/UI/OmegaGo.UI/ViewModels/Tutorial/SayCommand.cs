namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The "s [text]" command will replace the teacher's current line of text with [text].
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.ScenarioCommand" />
    internal class SayCommand : ScenarioCommand
    {
        private string _sayWhat;
        public SayCommand(string fullArgument)
        {
            this._sayWhat = fullArgument;
        }
        public override LoopControl Execute(Scenario scenario)
        {
            scenario.OnSenseiMessageChanged(this._sayWhat);
            return LoopControl.Continue;
        }
    }
}