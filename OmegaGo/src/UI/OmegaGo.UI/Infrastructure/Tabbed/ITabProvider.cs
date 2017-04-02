using System.Collections;
using System.Collections.Generic;
using MvvmCross.Core.ViewModels;

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
    }
}
