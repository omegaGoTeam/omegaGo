using System;
using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    public class ComplexLine : DialogueLine
    {
        private DialogueLine _then;
        private Func<Scenario, Position, bool> _isExpectedMove;
        private Action<Scenario> _onSpeak;
        public override bool NextButtonVisible => false;

        protected override void WhenShownToUser(Scenario scenario)
        {
            _onSpeak(scenario);
        }

        public ComplexLine(string line, Action<Scenario> onSpeak, Func<Scenario, Position, bool> isExpectedMove, DialogueLine then) : base(line)
        {
            this._onSpeak = onSpeak;
            this._isExpectedMove = isExpectedMove;
            this._then = then;
        }
        public override void SelectPosition(Scenario scenario, Position position)
        {
            if (_isExpectedMove(scenario, position))
            {
                _then.Speak(scenario);
            }
        }
    }
}