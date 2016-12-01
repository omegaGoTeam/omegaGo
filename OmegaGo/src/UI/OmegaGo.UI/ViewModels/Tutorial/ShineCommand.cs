using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The "shine [position]" command will set the given [position] as the current shining position that invites the user
    /// to click on it.
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.ScenarioCommand" />
    internal class ShineCommand : ScenarioCommand
    {
        private readonly Position _position;

        public ShineCommand(string position)
        {
            this._position = Position.FromIgsCoordinates(position);
        }

        public override LoopControl Execute(Scenario scenario)
        {
            scenario.OnSetShiningPosition(_position);
            return LoopControl.Continue;
        }
    }
}