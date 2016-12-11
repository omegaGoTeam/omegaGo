namespace OmegaGo.Core.Online.Igs.Structures
{
    class GameHeading
    {
        public readonly int GameNumber;
        public readonly string WhiteName;
        public readonly string BlackName;

        public GameHeading(int gameNumber, string whiteName, string blackName)
        {
            this.GameNumber = gameNumber;
            this.WhiteName = whiteName;
            this.BlackName = blackName;
        }
    }
}