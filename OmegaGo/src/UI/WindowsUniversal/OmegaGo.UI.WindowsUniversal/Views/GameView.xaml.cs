using System;
using Windows.UI.Xaml;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class GameView : TransparencyViewBase
    { 
        private DispatcherTimer _updateTimer;
        
        public GameView()
        {
            InitializeComponent();
        }

        public GameViewModel VM => (GameViewModel)ViewModel;

        public override string WindowTitle => Localizer.Game;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Game.png");

        private void TransparencyViewBase_Unloaded(object sender, RoutedEventArgs e)
        {
            _updateTimer.Tick -= UpdateTimer_Tick;
            VM.Unload();
        }

        private void TransparencyViewBase_Loaded(object sender, RoutedEventArgs e)
        {
            _updateTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(100)};
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
