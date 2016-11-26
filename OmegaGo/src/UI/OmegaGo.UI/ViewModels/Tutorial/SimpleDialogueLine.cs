using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    class SimpleDialogueLine : DialogueLine
    {
        private DialogueLine _nextLine;

        protected override void WhenShownToUser(Scenario scenario)
        {
            // Do nothing.
        }
        public override void Next(Scenario scenario)
        {
            if (_nextLine == null) throw new InvalidOperationException("The next line was not set.");
            _nextLine.Speak(scenario);
        }

        public SimpleDialogueLine(string line) : base(line)
        {
            
        }
        public SimpleDialogueLine(string line, DialogueLine then) : base(line)
        {
            _nextLine = then;
        }
        public SimpleDialogueLine Then(DialogueLine then)
        {
            if (then == null) throw new ArgumentNullException(nameof(then));
            if (_nextLine != null) throw new InvalidOperationException("The next line is already set.");
            this._nextLine = then;
            return this;
        }
    }
}
