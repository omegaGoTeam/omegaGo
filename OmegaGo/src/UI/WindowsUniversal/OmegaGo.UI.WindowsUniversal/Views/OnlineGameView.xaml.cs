using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;
using OmegaGo.UI.ViewModels;
using System;
using Windows.UI.Xaml;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace OmegaGo.UI.WindowsUniversal.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OnlineGameView : TransparencyViewBase
    {
        private DispatcherTimer _updateTimer;

        public OnlineGameView()
        {
            this.InitializeComponent();
        }

        public OnlineGameViewModel VM => (OnlineGameViewModel)ViewModel;

        public override string TabTitle => Localizer.Game;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Game.png");

        private void TransparencyViewBase_Unloaded(object sender, RoutedEventArgs e)
        {
            _updateTimer.Tick -= UpdateTimer_Tick;
            VM.Unload();
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

        private void DebugFill(object sender, RoutedEventArgs e)
        {
            for (int x = 1; x < VM.BoardViewModel.BoardControlState.BoardWidth; x += 3)
            {
                for (int xi = x; xi <= x + 1; xi++)
                {
                    for (int y = 1; y < VM.BoardViewModel.BoardControlState.BoardHeight - 1; y += 1)
                    {
                        (VM.Game.Controller.TurnPlayer.Agent as IHumanAgentActions)?.PlaceStone(new Position(
                            xi, y));

                    }
                }
            }
        }

        private void UpdateSystemLog(object sender, RoutedEventArgs e)
        {
            SystemLog.Text = VM.SystemLog;
        }
    }
}
