using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Models.Library
{
    public class AppDataLibraryItemViewModel : LibraryItemViewModel
    {
        public AppDataLibraryItemViewModel(LibraryItem item) : base(item)
        {
        }

        public override bool ShowCommands => true;

        public override LibraryItemKind Kind => LibraryItemKind.AppData;
    }
}
