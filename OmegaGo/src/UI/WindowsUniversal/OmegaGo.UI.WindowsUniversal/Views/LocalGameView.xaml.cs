﻿using OmegaGo.UI.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OmegaGo.UI.WindowsUniversal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocalGameView : TransparencyViewBase
    {
        public LocalGameView()
        {
            this.InitializeComponent();
            this.KeyUp += View_KeyUp;
        }
        
        public LocalGameViewModel VM => (LocalGameViewModel)ViewModel;

        public override string TabTitle => Localizer.LocalGame;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/LocalGame.png");
        
        private void focusButton_Click(object sender, RoutedEventArgs e)
        {
            AppShell.FocusModeOn = !AppShell.FocusModeOn;
        }

        private void View_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (VM.IsAnalyzeModeEnabled)
            {
                switch (e.Key)
                {
                    case Windows.System.VirtualKey.Left:
                        gameTreeControl.GoToParentNode();
                        break;
                    case Windows.System.VirtualKey.Right:
                        gameTreeControl.GoToFirstChildNode();
                        break;
                    case Windows.System.VirtualKey.Up:
                        gameTreeControl.GoToPreviousLevelNode();
                        break;
                    case Windows.System.VirtualKey.Down:
                        gameTreeControl.GoToNextLevelNode();
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Windows.System.VirtualKey.Left:
                        gameTimelineSlider.Value--;
                        break;
                    case Windows.System.VirtualKey.Right:
                        gameTimelineSlider.Value++;
                        break;
                }
            }
        }
    }
}
