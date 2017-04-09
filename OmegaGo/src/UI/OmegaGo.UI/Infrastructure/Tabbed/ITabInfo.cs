using System;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Infrastructure.Tabbed
{
    /// <summary>
    /// Represents a tab
    /// </summary>
    public interface ITabInfo
    {
        /// <summary>
        /// Tab's unique ID
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Title of the tab
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets the tab's current position
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Gets or sets if the tab is currently blinking to notify the user
        /// </summary>
        bool IsBlinking { get; set; }
        
        /// <summary>
        /// Tag
        /// </summary>
        object Tag { get; set; }   
        
        /// <summary>
        /// Currently displayed view model on this tab
        /// </summary>
        ViewModelBase CurrentViewModel { get; }
    }
}