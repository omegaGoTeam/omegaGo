using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Files
{
    public class FileInfo
    {
        public FileInfo(string name, ulong size, DateTimeOffset lastModified)
        {
            Name = name;
            Size = size;
            LastModified = lastModified;
        }
        
        public string Name { get; }

        public ulong Size { get; }

        public DateTimeOffset LastModified { get; }
    }
}
