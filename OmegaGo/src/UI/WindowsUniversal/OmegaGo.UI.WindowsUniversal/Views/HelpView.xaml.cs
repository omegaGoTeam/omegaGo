using OmegaGo.UI.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OmegaGo.UI.Services;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class HelpView : TransparencyViewBase
    {     
        public HelpView()
        {
            this.InitializeComponent();
        }

        public HelpViewModel VM => (HelpViewModel)this.ViewModel;

        public override string TabTitle => Localizer.Help;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Help.png");

        private void OpenCloseHelp(object sender, RoutedEventArgs e)
        {
            this.HelpSplitView.IsPaneOpen = !this.HelpSplitView.IsPaneOpen;
        }

        private async void WebView_OnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
        }
    }
}
