namespace OmegaGo.Core.Online.Igs.Structures
{
    internal class ScoreLine
    {
        public string White { get; private set; }
        public string Black { get; private set; }
        public float BlackScore { get; private set; }
        public float WhiteScore { get; private set; }
        public int GameId { get; set; }

        public ScoreLine(string white, string black, float blackScore, float whiteScore, int gameId)
        {
            this.White = white;
            this.Black = black;
            this.BlackScore = blackScore;
            this.WhiteScore = whiteScore;
            this.GameId = gameId;
        }
    }
}