using OmegaGo.Core.Time.Canadian;

namespace OmegaGo.Core.Online.Igs.Structures
{
    class GameHeading
    {
        public readonly int GameNumber;
        public readonly string WhiteName;
        public readonly string BlackName;
        public readonly CanadianTimeInformation BlackTimeRemaining;
        public readonly CanadianTimeInformation WhiteTimeRemaining;

        public GameHeading(int gameNumber, string whiteName, string blackName, CanadianTimeInformation blackTime, CanadianTimeInformation whiteTime)
        {
            this.GameNumber = gameNumber;
            this.WhiteName = whiteName;
            this.BlackName = blackName;
            this.WhiteTimeRemaining = whiteTime;
            this.BlackTimeRemaining = blackTime;
        }
    }
}