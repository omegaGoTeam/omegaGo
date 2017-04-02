using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.WindowsUniversal.UserControls.Navigation;

namespace OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed
{
    /// <summary>
    /// Manages the tabbed UI, acts as a view model for the TabbedUIControl
    /// </summary>
    public class TabManager : MvxNotifyPropertyChanged
    {
        /// <summary>
        /// Tabbed UI control managed by this tab manager
        /// </summary>
        private readonly TabbedUIControl _tabbedUIControl;

        private Tab _activeTab;

        /// <summary>
        /// Creates tab manager for a TabbedUIControl
        /// </summary>
        /// <param name="tabbedUIControl">Tabbed UI control</param>
        public TabManager( TabbedUIControl tabbedUIControl )
        {
            _tabbedUIControl = tabbedUIControl;            
        }

        /// <summary>
        /// Currently opened tabs
        /// </summary>
        public ObservableCollection<Tab> Tabs { get; } = new MvxObservableCollection<Tab>();

        /// <summary>
        /// Gets or sets the active tab
        /// </summary>
        public Tab ActiveTab
        {
            get { return _activeTab; }
            set { SetProperty(ref _activeTab, value); }
        }
    }
}
