using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The "require [position]" command will wait until the user clicks on the given [position].
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.ScenarioCommand" />
    internal class RequireCommand : ScenarioCommand
    {
        private readonly Position _position;
        public override void BoardClick(Position position, Scenario scenario)
        {
            if (position == this._position)
            {
                scenario.OnSetShiningPosition(Position.Undefined);
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