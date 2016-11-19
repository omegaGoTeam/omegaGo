using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.WindowsUWP.Views;
using Windows.UI.Xaml.Navigation;
using OmegaGo.UI.ViewModels;
using Windows.UI.Core;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public class ViewBase : MvxWindowsPage
    {        
        public ViewBase()
        {
            Loading += ViewBase_Loading;   
        }

        private void ViewBase_Loading( Windows.UI.Xaml.FrameworkElement sender, object args )
        {
            //set view model as Data Context by default
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            if ( Frame.CanGoBack )
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
                SystemNavigationManager.GetForCurrentView().BackRequested += BackRequested;
            }
            else
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
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


        private Localizer _localizer = null;

        /// <summary>
        /// Localizer for the ViewModel
        /// </summary>
        public Localizer Localizer => _localizer ?? ( _localizer = new Localizer() );
    }
}
