using OmegaGo.Core.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Online
{
    /// <summary>
    /// Contains metadata about a game that is or was in progress on the IGS server
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Game.GameInfo" />
    public class IgsGameInfo : GameInfo
    {
        public IgsGameInfo(
            PlayerInfo whitePlayerInfo,
            PlayerInfo blackPlayerInfo,
            GameBoardSize boardSize,
            RulesetType rulesetType,
            int numberOfHandicapStones,
            HandicapPlacementType handicapPlacementType,
            float komi,
            CountingType countingType,
            int igsIndex,
            int numberOfObservers,
            IgsConnection server) :
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
            NumberOfObservers = numberOfObservers;
            this.Server = server;
            IgsIndex = igsIndex;
        }
        public int MainTime { get; set; }
        public int ByoyomiPeriod { get; set; }

        public int IgsIndex { get; private set; }
        
        public IgsConnection Server { get; }
        public int NumberOfObservers { get; set; }

        public override string ToString()
        {
            return $"{White.Name}({White.Rank}) v. {Black.Name}({Black.Rank}) (" + NumberOfObservers + " observers)";
        }
    }
}
