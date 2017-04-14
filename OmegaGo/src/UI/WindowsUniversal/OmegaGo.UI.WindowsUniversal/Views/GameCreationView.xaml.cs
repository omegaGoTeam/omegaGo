using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class GameCreationView : TransparencyViewBase
    {
        private const int DotKeyValue = 190;
        private const int CommaKeyValue = 188;

        public GameCreationViewModel VM => (GameCreationViewModel)ViewModel;

        public GameCreationView()

        {
            InitializeComponent();
        }

        private void CompensationInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {             
            var isNumericInput = e.Key.ToString().StartsWith("Number") ||
                e.Key == VirtualKey.Stop ||
                e.Key == VirtualKey.Decimal ||
                e.Key == (VirtualKey)DotKeyValue ||
                e.Key == (VirtualKey)CommaKeyValue;
            if ( !isNumericInput )
            {
                e.Handled = true;
            }
        }

        private void CloseCustomBoardSizeFlyout(object sender, RoutedEventArgs e)
        {
            CustomBoardSizeFlyout.Hide();
        }

        private void CloseTimeControlFlyout(object sender, RoutedEventArgs e)
        {
            TimeControlFlyout.Hide();
        }
        private void CloseAgentPlayerFlyout(object sender, RoutedEventArgs e)
        {
            AgentFlyout.Hide();
        }

        private void CloseWhitePlayerFlyout(object sender, RoutedEventArgs e)
        {
            WhitePlayerFlyout.Hide();
        }

        private void CloseBlackPlayerFlyout(object sender, RoutedEventArgs e)
        {
            BlackPlayerFlyout.Hide();
        }
    }
}
