using System;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    public class ChoiceDialogueLine : DialogueLine
    {
        private readonly string _optionOneText;
        private readonly DialogueLine _optionOneLine;
        private readonly string _optionTwoText;
        private readonly DialogueLine _optionTwoLine;

        public ChoiceDialogueLine(string line, string optionOneText, DialogueLine optionOneLine, string optionTwoText, DialogueLine optionTwoLine)
            : base(line)
        {
            this._optionOneText = optionOneText;
            this._optionOneLine = optionOneLine;
            this._optionTwoText = optionTwoText;
            this._optionTwoLine = optionTwoLine;
        }

        protected override void WhenShownToUser(Scenario scenario)
        {
            scenario.OnSetChoices(this._optionOneText, this._optionTwoText);
        }

        public override bool NextButtonVisible => false;

        public override void SelectOption(int optionIndex, Scenario scenario)
        {
            if (optionIndex != 0 && optionIndex != 1)
                throw new ArgumentException("Only 2 options exist.", nameof(optionIndex));
            if (optionIndex == 0)
            {
                _optionOneLine.Speak(scenario);
            }
            else if (optionIndex == 1)
            {
                _optionTwoLine.Speak(scenario);
            }
        }
    }
}