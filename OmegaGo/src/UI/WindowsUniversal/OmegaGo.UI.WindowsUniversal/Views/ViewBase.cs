using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.WindowsUWP.Views;
using Windows.UI.Xaml.Navigation;
using OmegaGo.UI.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.WindowsUniversal.Infrastructure;
using OmegaGo.UI.WindowsUniversal.Infrastructure.Tabbed;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    /// <summary>
    /// Base for all regular views in OmegaGo
    /// </summary>
    public class ViewBase : MvxWindowsPage
    {
        private AppShell _appShell = null;
        private Localizer _localizer = null;       

        public ViewBase()
        {
            Loading += ViewBase_Loading;            
        }

        /// <summary>
        /// Title of the window (defaults to empty string)
        /// </summary>
        public virtual string TabTitle => string.Empty;

        /// <summary>
        /// Icon of the window (defaults to null)
        /// </summary>
        public virtual Uri TabIconUri => null;

        /// <summary>
        /// Gets the app shell for the current window
        /// </summary>
        public AppShell AppShell => _appShell ?? (_appShell = AppShell.GetForCurrentView());        

        /// <summary>
        /// Localizer for the ViewModel
        /// </summary>
        public Localizer Localizer => _localizer ?? (_localizer = new Localizer());

        /// <summary>
        /// Informs the view that its tab has been activated
        /// </summary>
        internal virtual void TabActivated() { }

        /// <summary>
        /// Informs the viewm that its tab has been deactivated
        /// </summary>
        internal virtual void TabDeactivated() { }

        /// <summary>
        /// Informs the view that its tab has been closed
        /// </summary>
        internal virtual void TabClosed() { }

        /// <summary>
        /// Returns the tab where this view is currently displayed or null
        /// </summary>
        /// <returns>Tab or null</returns>
        protected Tab GetViewsTab()
        {
            return AppShell.TabManager.Tabs.SingleOrDefault(t => t.Frame.Content == this);
        }

        /// <summary>
        /// Handles the loading phase of the View
        /// </summary>
        private void ViewBase_Loading(FrameworkElement sender, object args)
        {
            //notify the view model
            ( ViewModel as ViewModelBase )?.Appearing();
            //Set view model as Data Context by default
            DataContext = ViewModel;
        }

    }
}
