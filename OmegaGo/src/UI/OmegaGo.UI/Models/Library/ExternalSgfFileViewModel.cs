using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Models.Library
{
    public class ExternalSgfFileViewModel : LibraryItemViewModel
    {
        public ExternalSgfFileViewModel( string contents, LibraryItem item) : base(item)
        {
            Contents = contents;
        }

        public string Contents { get; }
        public override bool ShowCommands => false;
        public override LibraryItemKind Kind => LibraryItemKind.External;
    }
}
