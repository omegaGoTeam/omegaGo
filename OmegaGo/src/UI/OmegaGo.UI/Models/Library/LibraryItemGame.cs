namespace OmegaGo.UI.Models.Library
{
    /// <summary>
    /// Represents a single game in a library item
    /// </summary>
    public class LibraryItemGame
    {
        public LibraryItemGame( string name, int moveCount, string date, string blackName, string blackRank, string whiteName, string whiteRank, string comment)
        {
            Name = name;
            MoveCount = moveCount;
            Date = date;
            BlackName = blackName;
            BlackRank = blackRank;
            WhiteName = whiteName;
            WhiteRank = whiteRank;
            Comment = comment;
        }

        public string Name { get; set; }
        public int MoveCount { get; set; }
        public string Date { get; set; }
        public string BlackName { get; set; }
        public string BlackRank { get; set; }
        public string WhiteName { get; set; }
        public string WhiteRank { get; set; }
        public string Comment { get; set; }
    }
}