namespace OmegaGo.Core.Online.Igs.Structures
{
    class GameHeading
    {
        public int GameNumber;
        public string WhiteName;
        public string BlackName;

        public GameHeading(int gameNumber, string whiteName, string blackName)
        {
            this.GameNumber = gameNumber;
            this.WhiteName = whiteName;
            this.BlackName = blackName;
        }
    }
}