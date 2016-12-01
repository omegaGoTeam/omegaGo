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
using MvvmCross.Platform;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.WindowsUniversal.Infrastructure;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    /// <summary>
    /// Base for all regular views in OmegaGo
    /// </summary>
    public class ViewBase : MvxWindowsPage
    {
        public ViewBase()
        {
            Loading += ViewBase_Loading;            
        }

        private AppShell _appShell = null;

        /// <summary>
        /// Gets the app shell for the current window
        /// </summary>
        public AppShell AppShell => _appShell ?? (_appShell = AppShell.GetForCurrentView());


        private Localizer _localizer = null;

        /// <summary>
        /// Localizer for the ViewModel
        /// </summary>
        public Localizer Localizer => _localizer ?? (_localizer = new Localizer());

        /// <summary>
        /// Handles the loading phase of the View
        /// </summary>
        private void ViewBase_Loading(Windows.UI.Xaml.FrameworkElement sender, object args)
        {
            //Set view model as Data Context by default
            DataContext = ViewModel;
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (Frame.CanGoBack)
            {
                AppShell.TitleBarBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            }
            else
            {
                AppShell.TitleBarBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;                
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= BackRequested;
            base.OnNavigatingFrom(e);
        }

        private void BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                var vm = ViewModel as ViewModelBase;
                if (vm != null)
                {
                    e.Handled = true;
                    vm.GoBackCommand.Execute(null);
                }
            }
        }
    }
}
