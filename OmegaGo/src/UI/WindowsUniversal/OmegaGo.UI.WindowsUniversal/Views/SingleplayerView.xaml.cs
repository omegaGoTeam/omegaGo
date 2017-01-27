
using Windows.UI.Xaml.Controls;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Quests;
using OmegaGo.UI.Services.Tsumego;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class SingleplayerView : TransparencyViewBase
    {
        public SingleplayerViewModel VM => (SingleplayerViewModel)this.ViewModel;

        public SingleplayerView()
        {
            this.InitializeComponent();
        }
        
        private void TransparencyViewBase_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VM.Load();
        }

        private void ExchangeQuest_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VM.ExchangeQuest(((Button) sender).Tag as ActiveQuest);
        }
        private void TryThisNow_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VM.TryThisNow(((Button)sender).Tag as ActiveQuest);
        }
    }
}
