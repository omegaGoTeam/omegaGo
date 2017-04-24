using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;
using OmegaGo.UI.ViewModels;
using System;
using Windows.UI.Xaml;
using MvvmCross.Platform;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;
using OmegaGo.UI.Services.Dialogs;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OmegaGo.UI.WindowsUniversal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LocalGameView : TransparencyViewBase
    {
        private DispatcherTimer _updateTimer;

        public LocalGameView()
        {
            this.InitializeComponent();
        }
        
        public LocalGameViewModel VM => (LocalGameViewModel)ViewModel;
        
        public override string TabTitle => Localizer.LocalGame;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/LocalGame.png");

        private void TransparencyViewBase_Unloaded(object sender, RoutedEventArgs e)
        {
            _updateTimer.Tick -= UpdateTimer_Tick;
        }

        private void TransparencyViewBase_Loaded(object sender, RoutedEventArgs e)
        {
            _updateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, object e)
        {
            VM.BlackPortrait.Update();
            VM.WhitePortrait.Update();

        }
    }
}
