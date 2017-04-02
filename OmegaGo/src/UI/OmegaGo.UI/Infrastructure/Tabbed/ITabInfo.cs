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
        string Title { get; }
        
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