using OmegaGo.Core;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using OmegaGo.Core.Modes.LiveGame.Players.Agents;

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
            VM.Game.Controller.Main_Undo();
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
    }
}
