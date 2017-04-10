using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Game
{
    /// <summary>
    /// Contains metadata about a game that's in progress or used to be in progress on an online server. Online games have additional
    /// metadata in descendants of this class.
    /// </summary>
    /// <seealso cref="RemoteGameInfo"/>
    /// <seealso cref="IgsGameInfo"/>
    /// <seealso cref="KgsGameInfo"/>
    public class GameInfo
    {
        public GameInfo(PlayerInfo whitePlayerInfo, PlayerInfo blackPlayerInfo, GameBoardSize boardSize, RulesetType rulesetType, int numberOfHandicapStones, HandicapPlacementType handicapPlacementType, float komi, CountingType countingType)
        {
            White = whitePlayerInfo;
            Black = blackPlayerInfo;
            BoardSize = boardSize;
            RulesetType = rulesetType;
            NumberOfHandicapStones = numberOfHandicapStones;
            HandicapPlacementType = handicapPlacementType;
            Komi = komi;
            CountingType = countingType;
        }

        /// <summary>
        /// White player
        /// </summary>
        public PlayerInfo White { get; }

        /// <summary>
        /// Black player
        /// </summary>
        public PlayerInfo Black { get; }

        /// <summary>
        /// Board size
        /// </summary>
        public GameBoardSize BoardSize { get; set; }

        /// <summary>
        /// Ruleset type
        /// </summary>
        public RulesetType RulesetType { get; set; }

        /// <summary>
        /// Number of handicap stones
        /// </summary>
        public int NumberOfHandicapStones { get; set; }

        /// <summary>
        /// Handicap placement type
        /// </summary>
        public HandicapPlacementType HandicapPlacementType { get; set; }

        /// <summary>
        /// Komi compensation
        /// </summary>
        public float Komi { get; set; }

        /// <summary>
        /// Counting type
        /// </summary>
        public CountingType CountingType { get; set; }

        public override string ToString()
        {
            return "GameInfo: " + White + " v." + Black;
        }

    }
}
