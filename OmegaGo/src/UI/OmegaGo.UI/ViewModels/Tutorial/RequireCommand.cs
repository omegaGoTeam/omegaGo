using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    internal class RequireCommand : ScenarioCommand
    {
        private readonly Position _position;
        public override bool AllowsBoardClick => true;
        public override void BoardClick(Position position, Scenario scenario)
        {
            if (position == this._position)
            {
                scenario.ExecuteCommand();
            }
            else
            {
                // Show error to user.
            }
        }
        public RequireCommand(string position)
        {
            this._position = Position.FromIgsCoordinates(position);
        }

        public override LoopControl Execute(Scenario scenario)
        {
            return LoopControl.Stop;
        }
    }
}