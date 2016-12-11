namespace OmegaGo.Core.Online.Igs
{
    internal class ScoreLine
    {
        public string White { get; private set; }
        public string Black { get; private set; }
        public float BlackScore { get; private set; }
        public float WhiteScore { get; private set; }

        public ScoreLine(string white, string black, float blackScore, float whiteScore)
        {
            this.White = white;
            this.Black = black;
            this.BlackScore = blackScore;
            this.WhiteScore = whiteScore;
        }
    }
}