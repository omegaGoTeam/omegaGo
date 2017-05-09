
using OmegaGo.UI.ViewModels;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.WindowsUniversal.Views
{
    public sealed partial class KgsHomeView : MultiplayerLobbyViewBase
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

        private void CloseDetail(object sender, RoutedEventArgs e)
        {
            if (RoomsMasterDetail.ViewState == MasterDetailsViewState.Details)
            {
                VM.SelectedRoom = null;
            }
        }

        private void RoomsMasterDetail_OnViewStateChanged(object sender, MasterDetailsViewState e)
        {
            var grid = RoomsMasterDetail.FindDescendantByName("MasterPanel") as Grid;
            if (grid != null)
            {
                grid.Visibility = e == MasterDetailsViewState.Details ? Visibility.Collapsed : Visibility.Visible;
            }
            
        }
    }
}
