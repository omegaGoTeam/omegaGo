using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.ViewModels
{
    public class MultiplayerDashboardViewModel : ViewModelBase
    {
        private MvxCommand _navigateToIGSLobbyCommand;
        private MvxCommand _navigateToOGSLobbyCommand;

        public MvxCommand NavigateToIGSLobbyCommand => _navigateToIGSLobbyCommand ?? (_navigateToIGSLobbyCommand = new MvxCommand(() => ShowViewModel<IGSLobbyViewModel>()));
        public MvxCommand NavigateToOGSLobbyCommand => _navigateToOGSLobbyCommand ?? (_navigateToOGSLobbyCommand = new MvxCommand(() => ShowViewModel<OGSLobbyView>()));
        
        public MultiplayerDashboardViewModel()
        {
            
        }
    }
}
