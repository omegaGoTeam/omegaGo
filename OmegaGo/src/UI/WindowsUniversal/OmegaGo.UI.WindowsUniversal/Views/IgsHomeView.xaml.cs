
using OmegaGo.UI.ViewModels;
using System;

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
    }
}
