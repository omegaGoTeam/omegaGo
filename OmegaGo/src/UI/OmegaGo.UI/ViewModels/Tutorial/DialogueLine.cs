using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    public abstract class DialogueLine 
    {
        public string Line { get; private set; }
        public virtual string NextButtonText => "Next [K]";
        public virtual bool NextButtonVisible => true;
        public DialogueLine(string line)
        {
            Line = line;
        }
        public override string ToString()
        {
            return Line;
        }

        protected virtual void WhenShownToUser(Scenario scenario)
        {

        }

        public void Speak(Scenario scenario)
        {
            scenario.OnClearChoices();
            scenario.CurrentLine = this;
            // TODO Play an audio clip.
            scenario.OnNextButtonVisibilityChanged(NextButtonVisible);
            scenario.OnNewLineSpoken(this);
            this.WhenShownToUser(scenario);
        }

        public virtual void SelectOption(int optionIndex, Scenario scenario)
        {
            throw new InvalidOperationException("This dialogue line has no options to select from.");
        }

        public virtual void Next(Scenario scenario)
        {
            throw new InvalidOperationException("This dialogue line cannot respond to the 'Next' button.");
        }
        public virtual void SelectPosition(Scenario scenario, Position position)
        {
            // For most line, selecting a position doesn't do anything
        }
    }
}
