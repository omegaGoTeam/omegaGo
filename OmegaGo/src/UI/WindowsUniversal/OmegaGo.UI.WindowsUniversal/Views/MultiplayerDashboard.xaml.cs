
using OmegaGo.UI.ViewModels;
using System;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class MultiplayerDashboard : TransparencyViewBase
    {
        public MultiplayerDashboardViewModel VM => (MultiplayerDashboardViewModel)this.ViewModel;

        public MultiplayerDashboard()
        {
            this.InitializeComponent();
        }

        public override string WindowTitle => Localizer.OnlineGame;

        public override Uri WindowTitleIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Multiplayer.png");
    }
}
