namespace OmegaGo.UI.Models.Library
{
    /// <summary>
    /// Represents a single game in a library item
    /// </summary>
    public class LibraryItemGame
    {
        public LibraryItemGame(int moveCount, string date, string blackName, string blackRank, string whiteName, string whiteRank, string comment)
        {
            MoveCount = moveCount;
            Date = date;
            BlackName = blackName;
            BlackRank = blackRank;
            WhiteName = whiteName;
            WhiteRank = whiteRank;
            Comment = comment;
        }

        public int MoveCount { get; set; }
        public string Date { get; set; }
        public string BlackName { get; set; }
        public string BlackRank { get; set; }
        public string WhiteName { get; set; }
        public string WhiteRank { get; set; }
        public string Comment { get; set; }
    }
}