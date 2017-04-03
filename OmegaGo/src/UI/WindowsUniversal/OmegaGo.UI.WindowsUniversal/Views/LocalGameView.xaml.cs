using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.Local;
using OmegaGo.UI.ViewModels;
using System;
using Windows.UI.Xaml;
using MvvmCross.Platform;
using OmegaGo.Core.AI.Fuego;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;
using OmegaGo.UI.Services.Dialogs;

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
        
        public override string TabTitle => $"{VM.Game.Info.Black.Name} vs. {VM.Game.Info.White.Name}";

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/LocalGame.png");

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

        private async void SendFuegoCommand(object sender, RoutedEventArgs e)
        {
            foreach(var player in VM.Game.Controller.Players)
            {
                if (player.Agent is AiAgent)
                {
                    var fuego = (FuegoAI) ((AiAgent) player.Agent).AI;
                    var response = fuego.SendCommand(this.FuegoCommand.Text);
                    await Mvx.Resolve<IDialogService>().ShowAsync(response.Text, "Fuego response");
                }
            }
        }
    }
}
