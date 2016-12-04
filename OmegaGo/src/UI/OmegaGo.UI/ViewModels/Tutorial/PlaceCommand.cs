using System.Linq;
using OmegaGo.Core;

namespace OmegaGo.UI.ViewModels.Tutorial
{
    /// <summary>
    /// The "white [positions]...", "black [positions]..." and "clear [positions]..." commands will modify the game board by 
    /// changing the color of the given positions. For example, "black C3 C5" will replace whatever is at positions C3 and C5
    /// with black stones. The "clear" command will remove stones from the positions.
    /// </summary>
    /// <seealso cref="OmegaGo.UI.ViewModels.Tutorial.ScenarioCommand" />
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