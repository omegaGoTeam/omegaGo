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
    public sealed partial class ObserverGameView : TransparencyViewBase
    {
        private DispatcherTimer _updateTimer;
        
        public ObserverGameViewModel VM => (ObserverGameViewModel)ViewModel;

        public override string TabTitle => $"{VM.Game.Info.Black.Name} vs. {VM.Game.Info.White.Name}";

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Observe.png");


        public ObserverGameView()
        {
            this.InitializeComponent();
        }
        
        private void TransparencyViewBase_Unloaded(object sender, RoutedEventArgs e)
        {
            _updateTimer.Stop();
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
        
        private void UpdateSystemLog(object sender, RoutedEventArgs e)
        {
            SystemLog.Text = VM.SystemLog;
        }
    }
}
