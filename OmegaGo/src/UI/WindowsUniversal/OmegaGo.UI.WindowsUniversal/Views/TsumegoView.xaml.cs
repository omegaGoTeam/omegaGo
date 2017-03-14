using OmegaGo.Core;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    // ReSharper disable once UnusedMember.Global
    public sealed partial class TsumegoView
    {
        public TsumegoViewModel VM => (TsumegoViewModel)this.ViewModel;
        
        public TsumegoView()
        {
            this.InitializeComponent();
        }

        private void OpenCloseMenu(object sender, RoutedEventArgs e)
        {
            this.TsumegoSplitView.IsPaneOpen = !this.TsumegoSplitView.IsPaneOpen;
        }
    }
}
