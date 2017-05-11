namespace OmegaGo.UI.Models.Library
{
    /// <summary>
    /// Represents a single game in a library item
    /// </summary>
    public class LibraryItemGame
    {
        public LibraryItemGame(int moveCount, string date, string black, string white, string comment)
        {
            MoveCount = moveCount;
            Date = date;
            Black = black;
            White = white;
            Comment = comment;
        }

        public int MoveCount { get; set; }
        public string Date { get; set; }
        public string Black { get; set; }
        public string White { get; set; }
        public string Comment { get; set; }
    }
}