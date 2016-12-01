using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    internal abstract class ScenarioCommand
    {
        public abstract LoopControl Execute(Scenario scenario);
        public virtual bool AllowsButtonClick => false;
        public virtual bool AllowsOptionClick => false;
        public virtual bool AllowsBoardClick => false;

        public virtual void BoardClick(Position position, Scenario scenario)
        {

        }
        public virtual void OptionClick(int index, Scenario scenario)
        {
            
        }
        public virtual void ButtonClick(Scenario scenario)
        {
            
        }
    }
}