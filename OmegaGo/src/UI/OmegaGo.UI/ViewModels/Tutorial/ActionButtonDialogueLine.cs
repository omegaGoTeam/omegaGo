using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    class ActionButtonDialogueLine : DialogueLine
    {
        private Action<Scenario> _whenButtonClicked;

        public override string NextButtonText { get; }

        public ActionButtonDialogueLine(string line,
            string buttonCaption,
            Action<Scenario> whenButtonClicked) : base(line)
        {
            _whenButtonClicked = whenButtonClicked;
            this.NextButtonText = buttonCaption;
        }

        public override void Next(Scenario scenario)
        {
            _whenButtonClicked(scenario);
        }
    }
}
