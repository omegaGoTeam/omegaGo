using System;

namespace OmegaGo.UI.Services.Files
{
    /// <summary>
    /// Basic information about a file
    /// </summary>
    public class FileContentInfo : FileInfo
    {
        public FileContentInfo(string name, ulong size, DateTimeOffset lastModified, string contents) : base( name, size, lastModified )
        {
            Contents = contents;
        }

        public string Contents { get; }
    }
}