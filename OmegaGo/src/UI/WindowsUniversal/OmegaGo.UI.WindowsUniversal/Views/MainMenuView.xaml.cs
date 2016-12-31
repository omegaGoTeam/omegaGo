using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Navigation;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Infrastructure;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class MainMenuView : TransparencyViewBase
    {
        public MainMenuViewModel VM => (MainMenuViewModel)this.ViewModel;

        public MainMenuView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
        }

        private void ShowHideTips_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.BorderTips.Visibility = this.BorderTips.Visibility == Windows.UI.Xaml.Visibility.Collapsed
                ? Windows.UI.Xaml.Visibility.Visible
                : Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void GoFullScreen_Click( object sender, Windows.UI.Xaml.RoutedEventArgs e )
        {
            FullscreenModeManager.Toggle();
        }
    }
}
