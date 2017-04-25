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
        public ObserverGameViewModel VM => (ObserverGameViewModel)ViewModel;

        public override string TabTitle => Localizer.Observing;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Observe.png");


        public ObserverGameView()
        {
            this.InitializeComponent();
        }
        
    }
}
