using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    public class OnlineGameInfo : GameInfo
    {
        public OnlineGameInfo(
            PlayerInfo whitePlayerInfo,
            PlayerInfo blackPlayerInfo,
            GameBoardSize boardSize,
            RulesetType rulesetType,
            int numberOfHandicapStones,
            HandicapPlacementType handicapPlacementType,
            float komi,
            CountingType countingType) :
            base(
                whitePlayerInfo,
                blackPlayerInfo,
                boardSize,
                rulesetType,
                numberOfHandicapStones,
                handicapPlacementType,
                komi,
                countingType)
        {
        }

        public int NumberOfObservers { get; set; }
    }
}
