
using OmegaGo.UI.ViewModels;
using System;
using System.Linq;
using Windows.UI.Xaml;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class KgsHomeView : TransparencyViewBase
    {
        public KgsHomeViewModel VM => (KgsHomeViewModel)this.ViewModel;

        public KgsHomeView()
        {
            this.InitializeComponent();
        }

        private void TransparencyViewBase_Unloaded(object sender, RoutedEventArgs e)
        {
        }


        public override string TabTitle => Localizer.KgsServerCaption;

        public override Uri TabIconUri => new Uri("ms-appx:///Assets/Icons/TitleBar/Multiplayer.png");
    }
}
