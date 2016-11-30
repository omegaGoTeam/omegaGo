using MvvmCross.Core.ViewModels;
using OmegaGo.Core;
using OmegaGo.Core.Online.Igs;
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
        private IgsConnection _igsConnection;

        private ObservableCollection<Game> _onlineGames;
        private ObservableCollection<IgsUser> _onlinePlayers;

        private MvxCommand _refreshOnlineGamesCommand;
        private MvxCommand _refreshOnlinePlayersCommand;

        public ObservableCollection<Game> OnlineGames => _onlineGames;
        public ObservableCollection<IgsUser> OnlinePlayers => _onlinePlayers;

        public MvxCommand RefreshOnlineGamesCommand => _refreshOnlineGamesCommand ?? (_refreshOnlineGamesCommand = new MvxCommand(() => RefreshOnlineGames()));
        public MvxCommand RefreshOnlinePlayersCommand => _refreshOnlinePlayersCommand ?? (_refreshOnlinePlayersCommand = new MvxCommand(() => RefreshOnlinePlayers()));


        public MultiplayerDashboardViewModel()
        {
            _igsConnection = new IgsConnection();

            _onlineGames = new ObservableCollection<Game>();
            _onlinePlayers = new ObservableCollection<IgsUser>();

            _igsConnection.Connect();
        }

        private async void RefreshOnlineGames()
        {
            OnlineGames.Clear();
            var gamesInProgress = await _igsConnection.ListGamesInProgress();

            foreach (var onlineGame in gamesInProgress)
            {
                OnlineGames.Add(onlineGame);
            }
        }

        private async void RefreshOnlinePlayers()
        {
            OnlinePlayers.Clear();
            var onlinePlayers = await _igsConnection.ListOnlinePlayers();

            foreach (var onlinePlayer in onlinePlayers)
            {
                OnlinePlayers.Add(onlinePlayer);
            }
        }

        protected override void GoBack()
        {
            _igsConnection.Disconnect();
            base.GoBack();
        }
    }
}
