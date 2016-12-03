﻿using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;
using System.Collections.ObjectModel;

namespace OmegaGo.UI.ViewModels
{
    public class IGSLobbyViewModel : ViewModelBase
    {
        private IgsConnection _igsConnection;

        private ObservableCollection<Game> _onlineGames;
        private ObservableCollection<IgsUser> _onlinePlayers;

        private int _selectedOnlineGameIndex;

        private MvxCommand _refreshOnlineGamesCommand;
        private MvxCommand _refreshOnlinePlayersCommand;

        private MvxCommand _observeGameCommand;

        public ObservableCollection<Game> OnlineGames => _onlineGames;
        public ObservableCollection<IgsUser> OnlinePlayers => _onlinePlayers;

        public int SelectedOnlineGameIndex
        {
            get { return _selectedOnlineGameIndex; }
            set { SetProperty(ref _selectedOnlineGameIndex, value); ObserveGameCommand.RaiseCanExecuteChanged(); }
        }

        public MvxCommand RefreshOnlineGamesCommand => _refreshOnlineGamesCommand ?? (_refreshOnlineGamesCommand = new MvxCommand(() => RefreshOnlineGames()));
        public MvxCommand RefreshOnlinePlayersCommand => _refreshOnlinePlayersCommand ?? (_refreshOnlinePlayersCommand = new MvxCommand(() => RefreshOnlinePlayers()));

        public MvxCommand ObserveGameCommand => _observeGameCommand ?? (_observeGameCommand = new MvxCommand(() => ObserveGame()));


        public IGSLobbyViewModel()
        {
            _igsConnection = new IgsConnection();

            _onlineGames = new ObservableCollection<Game>();
            _onlinePlayers = new ObservableCollection<IgsUser>();

            _selectedOnlineGameIndex = -1;

            _igsConnection.Connect();
        }

        private void ObserveGame()
        {
            // Todo observed game do not have rules assigned, which causes a Null exeption in GameController.MakeMove
            Game observedGame = OnlineGames[SelectedOnlineGameIndex];
            observedGame.Ruleset = new ChineseRuleset(observedGame.BoardSize);
            Mvx.RegisterSingleton(observedGame);
            _igsConnection.StartObserving(observedGame);
            _igsConnection.RefreshBoard(observedGame);

            ShowViewModel<GameViewModel>();
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