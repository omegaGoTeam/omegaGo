using OmegaGo.Core;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class GameView : TransparencyViewBase
    { 
        private DispatcherTimer _updateTimer;
        
        public GameView()
        {
            this.InitializeComponent();
        }

        public GameViewModel VM => (GameViewModel)this.ViewModel;

        public override string WindowTitle => Localizer.Game;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Game.png");

        private void TransparencyViewBase_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _updateTimer.Tick -= UpdateTimer_Tick;
            VM.Unload();
        }

        private void TransparencyViewBase_Loaded(object sender, RoutedEventArgs e)
        {
            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(100);
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
                        (VM.Game.Controller.TurnPlayer.Agent as IHumanAgentActions)?.PlaceStone(new Core.Game.Position(
                            xi, y));

                    }
                }
            }
        }

        private void LifeDeathDone(object sender, RoutedEventArgs e)
        {
            VM.UiConnector.LifeDeath_RequestDone();
        }

        private void ResumeGame(object sender, RoutedEventArgs e)
        {
            VM.UiConnector.LifeDeath_ForceReturnToMain();
        }

        private void UndoDeathMarks(object sender, RoutedEventArgs e)
        {
            VM.UiConnector.LifeDeath_RequestUndoDeathMarks();
        }

        private void UpdateSystemLog(object sender, RoutedEventArgs e)
        {
            this.SystemLog.Text = this.VM.SystemLog;
        }
    }
}
