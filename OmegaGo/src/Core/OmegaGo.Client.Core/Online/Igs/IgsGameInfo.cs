using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// Contains metadata about a game that is or was in progress on the IGS server
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Game.GameInfo" />
    public class IgsGameInfo : RemoteGameInfo
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
            IgsIndex = igsIndex;
        }
        public int MainTime { get; set; }
        public int ByoyomiPeriod { get; set; }

        /// <summary>
        /// Unique identification of the game
        /// </summary>
        public int IgsIndex { get; private set; }

        public int NumberOfObservers { get; set; }

        public override string ToString()
        {
            return $"{White.Name}({White.Rank}) v. {Black.Name}({Black.Rank}) (" + NumberOfObservers + " observers)";
        }
    }
}
