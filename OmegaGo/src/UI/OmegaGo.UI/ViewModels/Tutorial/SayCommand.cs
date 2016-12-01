namespace OmegaGo.UI.ViewModels.Tutorial
{
    internal class SayCommand : ScenarioCommand
    {
        private string sayWhat;
        public SayCommand(string fullArgument)
        {
            this.sayWhat = fullArgument;
        }
        public override LoopControl Execute(Scenario scenario)
        {
            scenario.OnSenseiMessageChanged(this.sayWhat);
            return LoopControl.Continue;
        }
    }
}