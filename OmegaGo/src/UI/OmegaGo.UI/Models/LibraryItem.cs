using System;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.ViewModels
{
    public class LibraryItem
    {
        public LibraryItem(GameTree gameTree, string filename, int moveCount, string date, string black, string white,
            string comment, string content)
        {
            this.Content = content;
            this.GameTree = gameTree;
            this.Filename = filename;
            this.MoveCount = moveCount;
            this.Date = date;
            this.Black = black;
            this.White = white;
            this.Comment = comment;
        }

        public string Content { get; }
        public GameTree GameTree { get; }
        public string Filename { get; }
        public int MoveCount { get; }
        public string Date { get; }
        public string Black { get; }
        public string White { get; }
        public string Comment { get; }

        public override string ToString()
        {
            return this.Filename + Environment.NewLine + this.Comment;
        }
    }
}