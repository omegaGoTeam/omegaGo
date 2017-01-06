using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game
{
    public class GameInfo
    {
        public PlayerInfo White { get; set; }

        public PlayerInfo Black { get; set; }

        public GameBoardSize BoardSize { get; set; }

        public RulesetType RulesetType { get; set; }

        public int NumberOfHandicapStones { get; set; }

        public HandicapPlacementType HandicapPlacementType { get; set; }

        public float Komi { get; set; }

        public CountingType CountingType { get; set; }
    }
}
