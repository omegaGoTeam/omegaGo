using System;
using OmegaGo.Core.Game;

namespace OmegaGo.UI.ViewModels
{
    public class LibraryItem
    {
        public LibraryItem(GameTree gameTree, GameInfo gameInfo, string filename, int moveCount, string date, string black, string white,
            string comment, string content)
        {
            Content = content;
            GameTree = gameTree;
            GameInfo = gameInfo;
            Filename = filename;
            MoveCount = moveCount;
            Date = date;
            Black = black;
            White = white;
            Comment = comment;
        }

        public string Content { get; }

        public GameInfo GameInfo { get; }
        public GameTree GameTree { get; }
        public string Filename { get; }
        public int MoveCount { get; }
        public string Date { get; }
        public string Black { get; }
        public string White { get; }
        public string Comment { get; }

        public override string ToString()
        {
            return Filename + Environment.NewLine + Comment;
        }
    }
}