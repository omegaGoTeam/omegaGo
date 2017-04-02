using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// A territory map associates each position on a game board with a territory sign - either black, white or neutral. This can
    /// be displayed to the user during Life/Death Determination Phase.
    /// </summary>
    public class TerritoryMap
    {
        /// <summary>
        /// Creates a territory map.
        /// </summary>
        /// <param name="territoryMap">Territories on the game board</param>
        /// <param name="deadPositions">Dead stone positions</param>
        public TerritoryMap(Territory[,] territoryMap, List<Position> deadPositions)
        {
            Board = territoryMap;
            DeadPositions = deadPositions;
        }

        /// <summary>
        /// Gets the territory mapping, as explained in the class description.
        /// </summary>
        public Territory[,] Board { get; }

        /// <summary>
        /// Gets the positions that have been marked as dead.
        /// </summary>
        public IEnumerable<Position> DeadPositions { get; }

        public override string ToString()
        {
            return DeadPositions.Count() + " dead positions";
        }
    }
}
