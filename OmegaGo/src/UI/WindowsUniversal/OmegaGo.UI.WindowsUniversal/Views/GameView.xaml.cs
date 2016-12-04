using OmegaGo.Core;
using OmegaGo.UI.ViewModels;
using OmegaGo.UI.WindowsUniversal.Services.Game;
using System;
using Windows.Foundation;

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
    }
}
