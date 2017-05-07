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
            int numberOfObservers) :
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

        public int ByoyomiPeriod { get; set; }

        /// <summary>
        /// Unique identification of the game
        /// </summary>
        public int IgsIndex { get; private set; }

        public int NumberOfObservers { get; }
        /// <summary>
        /// Some games (professional games, mostly) on IGS have a name that's sent to a client that starts observing the game.
        /// </summary>
        public string GameName { get; set; }

        public override string ToString()
        {
            return $"{White.Name}({White.Rank}) v. {Black.Name}({Black.Rank}) (" + NumberOfObservers + " observers)";
        }

        public override bool Equals(object obj)
        {
            return (obj is IgsGameInfo) && this.Equals(obj as IgsGameInfo);
        }

        protected bool Equals(IgsGameInfo other)
        {
            return IgsIndex == other.IgsIndex && this.Black.Name == other.Black.Name &&
                   this.White.Name == other.White.Name;
        }

        public override int GetHashCode()
        {
            return IgsIndex;
        }
    }
}
