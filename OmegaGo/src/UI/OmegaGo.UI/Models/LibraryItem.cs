using System;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.Models
{
    /// <summary>
    /// Represents a library item.
    /// Public setters allow for JSON serialization
    /// </summary>
    public class LibraryItem
    {
        public LibraryItem()
        {
            
        }

        public LibraryItem(string fileName, int moveCount, string date, string black, string white, string comment, long fileSize, DateTimeOffset fileLastModified)
        {
            FileName = fileName;
            MoveCount = moveCount;
            Date = date;
            Black = black;
            White = white;
            Comment = comment;
            FileSize = fileSize;
            FileLastModified = fileLastModified;
        }

        public string FileName { get; set; }
        public int MoveCount { get; set; }
        public string Date { get; set; }
        public string Black { get; set; }
        public string White { get; set; }
        public string Comment { get; set; }

        public long FileSize { get; set; }
        public DateTimeOffset FileLastModified { get; set; }

        public override string ToString()
        {
            return FileName + Environment.NewLine + Comment;
        }
    }
}