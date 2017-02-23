using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Game
{
    public class TerritoryMap
    {
        public Territory[,] Board { get; }
        public GameBoardSize BoardSize { get; }

        public List<Position> DeadPositions = new List<Position>();

        public TerritoryMap(Territory[,] territoryMap, GameBoardSize size, List<Position> deadPositions)
        {
            this.BoardSize = size;
            this.Board = territoryMap;
            DeadPositions = deadPositions;
        }
    }
}
