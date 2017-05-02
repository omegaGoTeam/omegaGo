using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Common
{
    /// <summary>
    /// A common base class for metadata of a game that runs on an online server.
    /// </summary>
    public abstract class RemoteGameInfo : GameInfo
    {
        protected RemoteGameInfo(PlayerInfo whitePlayerInfo, PlayerInfo blackPlayerInfo, GameBoardSize boardSize, RulesetType rulesetType, int numberOfHandicapStones, HandicapPlacementType handicapPlacementType, float komi, CountingType countingType) : base(whitePlayerInfo, blackPlayerInfo, boardSize, rulesetType, numberOfHandicapStones, handicapPlacementType, komi, countingType)
        {
        }
    }
}
