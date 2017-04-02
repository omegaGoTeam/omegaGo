using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Infrastructure.Tabbed;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed
{
    /// <summary>
    /// Tab provider acts as a communication layer between tabs and shit
    /// </summary>
    internal class TabProvider : ITabProvider
    {
        private readonly TabManager _tabManager;

        /// <summary>
        /// Creates a tab provider using tab manager
        /// </summary>
        /// <param name="tabManager">Tab manager</param>
        public TabProvider( TabManager tabManager )
        {
            _tabManager = tabManager;
        }

        /// <summary>
        /// Reads current tabs from the UI
        /// </summary>
        public IEnumerable<ITabInfo> Tabs => _tabManager.Tabs;

        /// <summary>
        /// Gets the active tab from the UI
        /// </summary>
        public ITabInfo ActiveTab => _tabManager.ActiveTab;
    }
}
