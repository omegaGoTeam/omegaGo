using OmegaGo.UI.ViewModels;
using System;
using OmegaGo.UI.Services;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class HelpView : TransparencyViewBase
    {
        public HelpViewModel VM => (HelpViewModel)this.ViewModel;

        public HelpView()
        {
            this.InitializeComponent();
        }
       

        private void VM_WebViewContentChanged(object sender, string e)
        {
            WebView.NavigateToString(e);
        }

        public override string WindowTitle => Localizer.Help;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Help.png");

        private void TransparencyViewBase_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VM.WebViewContentChanged += VM_WebViewContentChanged; // TODO Martin : when unsubscribe?
            VM.NavigateToCurrentItem();
        }
    }
}
