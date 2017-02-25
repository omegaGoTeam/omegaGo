using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Mapping of territories on game board
    /// </summary>
    public class TerritoryMap
    {
        /// <summary>
        /// Creates a territory map
        /// </summary>
        /// <param name="territoryMap">Territories on the game board</param>
        /// <param name="deadPositions">Dead stone positions</param>
        public TerritoryMap(Territory[,] territoryMap, List<Position> deadPositions)
        {
            Board = territoryMap;
            DeadPositions = deadPositions;
        }

        /// <summary>
        /// Territories on the game board
        /// </summary>
        public Territory[,] Board { get; }

        /// <summary>
        /// Dead positions
        /// </summary>
        public IEnumerable<Position> DeadPositions { get; }
    }
}
