using System.Linq;
using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    internal class PlaceCommand : ScenarioCommand
    {
        private readonly StoneColor _stoneColor;
        private Position[] _positions;
        public PlaceCommand(StoneColor stoneColor, params string[] positions)
        {
            this._stoneColor = stoneColor;
            this._positions = positions.Select(Position.FromIgsCoordinates).ToArray();
        }

        public override LoopControl Execute(Scenario scenario)
        {
            foreach(Position position in this._positions)
            {
                scenario.PlaceStone(this._stoneColor, position);
            }
            return LoopControl.Continue;
        }
    }
}