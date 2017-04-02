using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Infrastructure.Tabbed;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed
{
    internal class TabsProvider : ITabsProvider
    {
        public TabsProvider()
        {            
        }

        public IEnumerable<ITabInfo> Tabs { get; }
        public ITabInfo ActiveTab { get; }
    }
}
