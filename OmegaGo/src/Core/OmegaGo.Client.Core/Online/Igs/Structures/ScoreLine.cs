namespace OmegaGo.Core.Online.Igs
{
    internal class ScoreLine
    {
        public string White { get; set; }
        public string Black { get; set; }
        public float BlackScore { get; set; }
        public float WhiteScore { get; set; }

        public ScoreLine(string white, string black, float blackScore, float whiteScore)
        {
            this.White = white;
            this.Black = black;
            this.BlackScore = blackScore;
            this.WhiteScore = whiteScore;
        }
    }
}