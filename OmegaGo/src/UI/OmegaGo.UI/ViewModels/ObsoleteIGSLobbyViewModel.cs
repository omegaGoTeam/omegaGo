using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;
using System.Collections.ObjectModel;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;

namespace OmegaGo.UI.ViewModels
{
    public class ObsoleteIGSLobbyViewModel : ViewModelBase
    {
        //private IgsConnection _igsConnection;

        //private ObservableCollection<ObsoleteGameInfo> _onlineGames;
        //private ObservableCollection<IgsUser> _onlinePlayers;

        //private int _selectedOnlineGameIndex;

        //private MvxCommand _refreshOnlineGamesCommand;
        //private MvxCommand _refreshOnlinePlayersCommand;

        //private MvxCommand _observeGameCommand;

        //public ObservableCollection<ObsoleteGameInfo> OnlineGames => _onlineGames;
        //public ObservableCollection<IgsUser> OnlinePlayers => _onlinePlayers;

        //public int SelectedOnlineGameIndex
        //{
        //    get { return _selectedOnlineGameIndex; }
        //    set { SetProperty(ref _selectedOnlineGameIndex, value); ObserveGameCommand.RaiseCanExecuteChanged(); }
        //}

        //public MvxCommand RefreshOnlineGamesCommand => _refreshOnlineGamesCommand ?? (_refreshOnlineGamesCommand = new MvxCommand(() => RefreshOnlineGames()));
        //public MvxCommand RefreshOnlinePlayersCommand => _refreshOnlinePlayersCommand ?? (_refreshOnlinePlayersCommand = new MvxCommand(() => RefreshOnlinePlayers()));

        //public MvxCommand ObserveGameCommand => _observeGameCommand ?? (_observeGameCommand = new MvxCommand(() => ObserveGame()));


        //public ObsoleteIGSLobbyViewModel()
        //{
        //    _igsConnection = new IgsConnection();

        //    _onlineGames = new ObservableCollection<ObsoleteGameInfo>();
        //    _onlinePlayers = new ObservableCollection<IgsUser>();

        //    _selectedOnlineGameIndex = -1;

        //    _igsConnection.ConnectAsync();
        //}

        //private void ObserveGame()
        //{
        //    // Todo observed game do not have rules assigned, which causes a Null exeption in GameController.MakeMove
        //    ObsoleteGameInfo observedGame = OnlineGames[ SelectedOnlineGameIndex ];
        //    observedGame.Ruleset = new ChineseRuleset( observedGame.BoardSize );

        //    IObsoleteGame game = new ObsoleteGame( observedGame, observedGame.GameController, null );

        //    Mvx.RegisterSingleton<IObsoleteGame>( game );
        //    _igsConnection.StartObserving(observedGame);
        //    _igsConnection.RefreshBoard(observedGame);

        //    ShowViewModel<GameViewModel>();
        //}

        //private async void RefreshOnlineGames()
        //{
        //    OnlineGames.Clear();
        //    var gamesInProgress = await _igsConnection.ListGamesInProgressAsync();

        //    foreach (var onlineGame in gamesInProgress)
        //    {
        //        OnlineGames.Add(onlineGame);
        //    }
        //}

        //private async void RefreshOnlinePlayers()
        //{
        //    OnlinePlayers.Clear();
        //    var onlinePlayers = await _igsConnection.ListOnlinePlayersAsync();

        //    foreach (var onlinePlayer in onlinePlayers)
        //    {
        //        OnlinePlayers.Add(onlinePlayer);
        //    }
        //}

        //protected override async void GoBack()
        //{
        //    await _igsConnection.DisconnectAsync();
        //    base.GoBack();
        //}
    }
}
