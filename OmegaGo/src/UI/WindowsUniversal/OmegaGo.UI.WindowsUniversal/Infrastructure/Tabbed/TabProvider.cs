using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed
{
    /// <summary>
    /// Tab provider acts as a communication layer between tabs and shit
    /// </summary>
    internal class TabProvider : ITabProvider
    {
        /// <summary>
        /// Reads current tabs from the UI
        /// </summary>
        public IEnumerable<ITabInfo> Tabs => GetTabManager().Tabs;

        /// <summary>
        /// Gets the active tab from the UI
        /// </summary>
        public ITabInfo ActiveTab => GetTabManager().ActiveTab;

        /// <summary>
        /// Shows a view model using tab manager
        /// </summary>
        /// <param name="request">View model request</param>
        /// <param name="tabNavigationType">Tab navigation type</param>
        /// <returns>Used tab</returns>
        public ITabInfo ShowViewModel(MvxViewModelRequest request, TabNavigationType tabNavigationType)
            => GetTabManager().ProcessViewModelRequest(request, tabNavigationType);

        public bool SwitchToTab(ITabInfo tab)
            => GetTabManager().SwitchToTab(tab);


        public bool CloseTab(ITabInfo tab)
            => GetTabManager().CloseTab(tab);

        /// <summary>
        /// Returns the tab for a given view model
        /// </summary>
        /// <param name="viewModel">View model</param>
        /// <returns>Tab or null</returns>
        public ITabInfo GetTabForViewModel(ViewModelBase viewModel)
            => GetTabManager().Tabs.FirstOrDefault(t => t.CurrentViewModel == viewModel);

        /// <summary>
        /// Retrieves the tab manager for current view
        /// </summary>
        /// <returns>Tab manager</returns>
        private TabManager GetTabManager() => AppShell.GetForCurrentView().TabManager;
    }
}
