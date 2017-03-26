﻿using OmegaGo.UI.ViewModels;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class GameCreationView : TransparencyViewBase
    {
        private const int DotKeyValue = 190;
        private const int CommaKeyValue = 188;

        public GameCreationViewModel VM => (GameCreationViewModel)this.ViewModel;

        public GameCreationView()

        {
            this.InitializeComponent();
        }

        private void CompensationInput_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
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
            this.CustomBoardSizeFlyout.Hide();
        }

        private void CloseTimeControlFlyout(object sender, RoutedEventArgs e)
        {
            this.TimeControlFlyout.Hide();
        }

        private void CloseWhitePlayerFlyout(object sender, RoutedEventArgs e)
        {
            this.WhitePlayerFlyout.Hide();
        }

        private void CloseBlackPlayerFlyout(object sender, RoutedEventArgs e)
        {
            this.BlackPlayerFlyout.Hide();
        }
    }
}
