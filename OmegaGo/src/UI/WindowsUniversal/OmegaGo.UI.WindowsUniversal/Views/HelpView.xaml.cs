using OmegaGo.UI.ViewModels;
using System;
using Windows.UI.Xaml;
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

        public override string WindowTitle => Localizer.Help;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Help.png");

        private void VM_WebViewContentChanged(object sender, string e)
        {
            WebView.NavigateToString(e);
        }

        private void OpenCloseHelp(object sender, RoutedEventArgs e)
        {
            this.helpSplitView.IsPaneOpen = !this.helpSplitView.IsPaneOpen;
        }
    }
}
