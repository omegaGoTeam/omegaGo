
using OmegaGo.UI.ViewModels;
using System;
using Windows.UI.Xaml;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class IgsHomeView : TransparencyViewBase
    {
        public IgsHomeViewModel VM => (IgsHomeViewModel)this.ViewModel;

        public IgsHomeView()
        {
            this.InitializeComponent();
        }

        private async void HyperlinkButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var pandanetUri = new Uri(@"http://pandanet-igs.com/igs_users/register");
            await Windows.System.Launcher.LaunchUriAsync(pandanetUri);

        }

        private void IgsHomeLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VM.Initialize();
        }

        private void IgsHomeUnloaded(object sender, RoutedEventArgs e)
        {
            VM.Deinitialize();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // temporary, maybe permanent
            VM.LoginScreenVisible = false;
        }

        private void ChangeUser_Click(object sender, RoutedEventArgs e)
        {
            VM.LoginScreenVisible = true;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            VM.Logout();
        }
    }
}
