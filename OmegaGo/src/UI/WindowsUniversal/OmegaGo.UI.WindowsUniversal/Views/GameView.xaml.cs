using OmegaGo.Core;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;
using OmegaGo.Core.Modes.LiveGame.Players.AI;
using OmegaGo.Core.Modes.LiveGame.Players.Local;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class GameView : TransparencyViewBase
    {
        public GameViewModel VM => (GameViewModel)this.ViewModel;
        
        public GameView()
        {
            this.InitializeComponent();
        }

        public override string WindowTitle => Localizer.Game;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Game.png");

        private void TransparencyViewBase_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            updateTimer.Tick -= UpdateTimer_Tick;
            VM.Unload();
        }

        private void UndoLocally(object sender, RoutedEventArgs e)
        {
            if (VM.Game.Controller.IsOnlineGame)
            {

            }
            else
            {
                VM.Game.Controller.Main_Undo();
            }
        }

        private void ClickPass(object sender, RoutedEventArgs e)
        {
            if (VM.Game?.Controller?.TurnPlayer?.IsHuman ?? false)
            {
                (VM.Game.Controller.TurnPlayer.Agent as IHumanAgentActions)?.Pass();
            }
        }

        private void ClickResign(object sender, RoutedEventArgs e)
        {
            if (VM.Game?.Controller?.TurnPlayer?.IsHuman ?? false)
            {
                (VM.Game.Controller.TurnPlayer.Agent as IHumanAgentActions)?.Resign();
            }
            // TODO make this possible even on opponent's turn, and ask for confirmation first
        }

        private DispatcherTimer updateTimer;
        private void TransparencyViewBase_Loaded(object sender, RoutedEventArgs e)
        {
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start(); 
        }

        private void UpdateTimer_Tick(object sender, object e)
        {
            VM.BlackPortrait.Update();
            VM.WhitePortrait.Update();
        }

        private void DebugFill(object sender, RoutedEventArgs e)
        {
            for (int x = 1; x < VM.BoardViewModel.BoardControlState.BoardWidth; x+=3)
            {
                for (int xi= x; xi <= x + 1; xi++)
                {
                    for (int y = 1; y < VM.BoardViewModel.BoardControlState.BoardHeight - 1; y+= 1)
                    {
                        (VM.Game.Controller.TurnPlayer.Agent as IHumanAgentActions)?.PlaceStone(new Core.Game.Position(
                            xi, y));

                    }
                }
            }
        }

        private async void LifeDeathDone(object sender, RoutedEventArgs e)
        {
            var game = VM.Game;
            if (game.Controller.IsOnlineGame)
            {
                await game.Controller.Server.Commands.LifeDeathDone(game.Controller.RemoteInfo);
            }
            else
            {
                foreach (var player in game.Controller.Players)
                {
                    if (player.Agent is HumanAgent || player.Agent is AiAgent)
                    {
                        game.Controller.LifeDeath_Done(player);
                    }
                }
            }
        }

        private void ResumeGame(object sender, RoutedEventArgs e)
        {
            if (VM.Game.Controller.IsOnlineGame)
            {
                // unsupported on IGS
            }
            else
            {
                VM.Game.Controller.LifeDeath_Resume();
            }
        }

        private async void UndoDeathMarks(object sender, RoutedEventArgs e)
        {
            var liveGame = VM.Game;
            if (liveGame.Controller.IsOnlineGame)
            {
                var onlineGame = (RemoteGame)liveGame;
                await liveGame.Controller.Server.Commands.UndoLifeDeath(onlineGame.RemoteInfo);
            }
            else
            {
                var controller = VM.Game.Controller;
                controller.LifeDeath_UndoPhase();
            }
        }
    }
}
