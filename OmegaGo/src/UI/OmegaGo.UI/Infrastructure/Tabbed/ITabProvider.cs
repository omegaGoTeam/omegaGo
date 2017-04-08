using System.Collections;
using System.Collections.Generic;
using MvvmCross.Core.ViewModels;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Infrastructure.Tabbed
{
    /// <summary>
    /// Provides read-only access to currently opened tabs in the UI project
    /// </summary>
    public interface ITabProvider
    {
        /// <summary>
        /// Currently opened tabs
        /// </summary>
        IEnumerable<ITabInfo> Tabs { get; }

        /// <summary>
        /// Active tab
        /// </summary>
        ITabInfo ActiveTab { get; }

        /// <summary>
        /// Shows view model in tab-based UI
        /// </summary>
        /// <returns>Used tab</returns>
        ITabInfo ShowViewModel(MvxViewModelRequest request, TabNavigationType tabNavigationType);

        /// <summary>
        /// Switches to the given tab
        /// </summary>
        /// <param name="tab">Tab to switch to</param>
        /// <returns>Was switch successful?</returns>
        bool SwitchToTab(ITabInfo tab);

        /// <summary>
        /// Closes the given tab
        /// </summary>
        /// <param name="tab">Tab to be closed</param>
        /// <returns>Was closing the tab successful?</returns>
        bool CloseTab(ITabInfo tab);

        /// <summary>
        /// Gets the tab for a given viewModel
        /// </summary>
        /// <param name="viewModel">View model to get tab for</param>
        /// <returns>Tab or null</returns>
        ITabInfo GetTabForViewModel(ViewModelBase viewModel);
    }
}
