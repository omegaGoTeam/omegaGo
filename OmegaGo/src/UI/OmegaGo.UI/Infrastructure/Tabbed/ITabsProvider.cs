using System.Collections;
using System.Collections.Generic;

namespace OmegaGo.UI.Infrastructure.Tabbed
{
    /// <summary>
    /// Provides read-only access to currently opened tabs in the UI project
    /// </summary>
    public interface ITabsProvider
    {
        /// <summary>
        /// Currently opened tabs
        /// </summary>
        IEnumerable<ITabInfo> Tabs { get; }

        /// <summary>
        /// Active tab
        /// </summary>
        ITabInfo ActiveTab { get; }        
    }
}
