using System;

namespace OmegaGo.UI.Models.Library
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

        public LibraryItem(string fileName, LibraryItemGame[] games, ulong fileSize, DateTimeOffset fileLastModified)
        {
            FileName = fileName;
            Games = games;
            FileSize = fileSize;
            FileLastModified = fileLastModified;
        }

        public string FileName { get; set; }

        public LibraryItemGame[] Games { get; set; }

        public ulong FileSize { get; set; }

        public DateTimeOffset FileLastModified { get; set; }
    }
}