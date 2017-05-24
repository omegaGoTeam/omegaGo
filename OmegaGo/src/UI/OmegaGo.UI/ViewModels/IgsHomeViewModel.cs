using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class IgsHomeViewModel : ViewModelBase
    {
        private readonly IGameSettings _settings;

#if DEBUG
        private string _usernameFilter = "Soothie";
#else
        private string _usernameFilter = "";
#endif

        private ICommand _logoutCommand;
        private ICommand _observeCommand;
        private ICommand _refreshCommand;
        private ICommand _challengePlayerCommand;

        private int _selectedViewIndex;

        public IgsHomeViewModel(IGameSettings settings)
        {
            _settings = settings;
            LoginForm = new IgsLoginForm(Localizer, _settings);
            LoginForm.LoginClick +=  LoginForm_LoginClick;

        }

        /// <summary>
        /// Logs the user out
        /// </summary>
        public ICommand LogoutCommand => _logoutCommand ?? (_logoutCommand = new MvxCommand(Logout));

        /// <summary>
        /// Observe game
        /// </summary>
        public ICommand ObserveCommand => _observeCommand ?? (_observeCommand = new MvxAsyncCommand<IgsGameInfo>(ObserveGame));

        /// <summary>
        /// Refreshes the current view
        /// </summary>
        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new MvxAsyncCommand(RefreshAsync));

        /// <summary>
        /// Selected view index
        /// </summary>
        public int SelectedViewIndex
        {
            get { return _selectedViewIndex; }
            set { SetProperty(ref _selectedViewIndex, value); }
        }

        private async void LoginForm_LoginClick(object sender, LoginEventArgs e)
        {
            await AttemptLoginCommand(e.Username, e.Password);
        }

        public async Task Initialize()
        {
            LoginForm.FormVisible = Connections.Igs.Composure != IgsComposure.Ok ||
                                    Connections.Igs.CurrentLoginPhase != IgsLoginPhase.Done;
            Connections.Igs.Events.PersonalInformationUpdate += Pandanet_PersonalInformationUpdate;
            Connections.Igs.Events.Disconnected += Events_Disconnected;
            Connections.Igs.Events.LoginPhaseChanged += Events_LoginPhaseChanged;
            Connections.Igs.Events.LoginComplete += OnLoginComplete;
            if (LoginForm.FormVisible && Connections.Igs.Composure != IgsComposure.Disconnected)
            {
                LoginForm.FormEnabled = false;
                LoginForm.LoginErrorMessage = Localizer.Igs_LoginAlreadyInProgress;
                LoginForm.LoginErrorMessageVisible = true;
            }
            else if (Connections.Igs.LoggedIn)
            {
                await EnterIgsLobbyLoggedIn();
            }
        }

        public event Action RefreshUsersComplete;
        public event Action RefreshGamesComplete;

        public override Task<bool> CanCloseViewModelAsync()
        {
            Connections.Igs.Events.PersonalInformationUpdate -= Pandanet_PersonalInformationUpdate;
            Connections.Igs.Events.LoginComplete -= OnLoginComplete;
            Connections.Igs.Events.Disconnected -= Events_Disconnected;
            Connections.Igs.Events.LoginPhaseChanged -= Events_LoginPhaseChanged;
            return base.CanCloseViewModelAsync();
        }


        private void Events_LoginPhaseChanged(object sender, IgsLoginPhase e)
        {
            LoginForm.FormEnabled = false;
            LoginForm.LoginErrorMessage = Localizer.GetString("IgsLoginPhase_" + e); ;
        }

        private void Events_Disconnected(object sender, EventArgs e)
        {
            LoginForm.FormEnabled = true;
            LoginForm.FormVisible = true;
            LoginForm.LoginErrorMessage = Localizer.YouHaveBeenDisconnected;
            LoginForm.LoginErrorMessageVisible = true;

        }

        private async void OnLoginComplete(object sender, bool success)
        {
            if (success)
            {
                LoginForm.FormVisible = false;
                LoginForm.LoginErrorMessageVisible = false;
                await EnterIgsLobbyLoggedIn();
            }
            else
            {
                LoginForm.FormEnabled = true;
                LoginForm.LoginErrorMessage = Localizer.LoginFailed;
                LoginForm.LoginErrorMessageVisible = true;
            }
        }

        private async Task EnterIgsLobbyLoggedIn()
        {
            allUsers = Connections.Igs.Data.OnlineUsers;
            ObservableGames = new ObservableCollection<IgsGameInfo>(Connections.Igs.Data.GamesInProgress);
            RefillChallengeableUsersFromAllUsers();
            RaisePropertyChanged(nameof(LoggedInUser));
            LoginForm.FormVisible = false;
            LoginForm.FormEnabled = true;
            LoginForm.LoginErrorMessageVisible = false;
            await Connections.Igs.Commands.RequestPersonalInformationUpdate(Connections.Igs.Username);
        }

        private async Task ObserveGame( IgsGameInfo selectedGame )
        {
            if (selectedGame == null) return;
            ShowProgressPanel(Localizer.Igs_InitiatingObservationOfAGame);
            var onlinegame = await Connections.Igs.Commands.StartObserving(selectedGame);
            if (onlinegame == null)
            {
                // TODO (future work)  Petr: error report
            }
            else
            {
                Mvx.RegisterSingleton<IGame>(onlinegame);
                OpenInNewActiveTab<ObserverGameViewModel>();
            }
            IsWorking = false;
        }

        private async Task RefreshAsync()
        {
            switch (SelectedViewIndex)
            {
                case 0:
                {
                    await RefreshGames();
                    RefreshGamesComplete?.Invoke();
                    break;
                }
                case 1:
                {
                    await RefreshUsers();
                    RefreshUsersComplete?.Invoke();
                    break;
                }
                default:
                {                    
                    return;
                }
            }
        }

        public LoginFormViewModel LoginForm { get; }


        //***************************************************************
        // STATUS BAR
        public string LoggedInUser
        {
            get
            {
                if (Connections.Igs.LoggedIn)
                {
                    return Connections.Igs.Username;
                }
                return Localizer.NotLoggedIn;
            }
        }


        private bool _incomingCheckboxChange;
        private string _progressPanelText = "";

        private bool _humanLookingForGame;
        public bool HumanLookingForGame
        {
            get { return _humanLookingForGame; }
            set {
                SetProperty(ref _humanLookingForGame, value);
                if (!_incomingCheckboxChange)
                {
                    ToggleHumanLookingForGameTo(value);
                }
            }
        }

       

        private bool _humanRefusingAllGames;
        public bool HumanRefusingAllGames
        {
            get { return _humanRefusingAllGames; }
            set {
                SetProperty(ref _humanRefusingAllGames, value);
                if (!_incomingCheckboxChange)
                    ToggleRefusingAllGamesTo(value);
            }
        }

        private async void ToggleRefusingAllGamesTo(bool value)
        {
            ShowProgressPanel(Localizer.TogglingOpenness);
            await Connections.Igs.Commands.ToggleAsync("open", !value);
            IsWorking = false;
        }
        private async void ToggleHumanLookingForGameTo(bool value)
        {
            ShowProgressPanel(Localizer.TogglingLFG);
            await Connections.Igs.Commands.ToggleAsync("looking", value);
            IsWorking = false;
        }

        public string ProgressPanelText
        {
            get { return _progressPanelText; }
            set { SetProperty(ref _progressPanelText, value); }

        }

        public string UsernameFilter
        {
            get { return _usernameFilter; }
            set {
                SetProperty(ref _usernameFilter, value);
                RefillChallengeableUsersFromAllUsers();
            }
        }
        public async Task AttemptLoginCommand(string username, string password)
        {
            LoginForm.LoginErrorMessageVisible = true;
            IsWorking = true;
            ProgressPanelText = Localizer.Igs_ConnectingToPandanet;
            LoginForm.FormEnabled = false;
            LoginForm.LoginErrorMessage = Localizer.Igs_ConnectingToPandanet;
            if (!Connections.Igs.ConnectionEstablished)
            {
                bool success = await Connections.Igs.ConnectAsync();
                if (!success)
                {
                    LoginForm.LoginErrorMessage = Localizer.Igs_ConnectionError;
                    LoginForm.FormEnabled = true;
                    LoginForm.LoginErrorMessageVisible = true;
                    IsWorking = false;
                    return;
                }
            }


            ProgressPanelText = String.Format(Localizer.Igs_LoggingInAs, username);
            LoginForm.LoginErrorMessage = String.Format(Localizer.Igs_LoggingInAs, username);
            bool loginSuccess = await Connections.Igs.LoginAsync(username, password);
            LoginForm.FormEnabled = true;
            if (loginSuccess)
            {
                RaisePropertyChanged(nameof(LoggedInUser));
                await EnterIgsLobbyLoggedIn();
            }
            else
            {
                LoginForm.LoginErrorMessage = Localizer.WrongCredentials;
                LoginForm.LoginErrorMessageVisible = true;
            }
            IsWorking = false;
        }

        public async void Logout()
        {
            if (!Connections.Igs.LoggedIn)
            {
                Debug.WriteLine("You are not logged in.");
            }
            IsWorking = true;
            ProgressPanelText = Localizer.Igs_Disconnecting;
            await Connections.Igs.DisconnectAsync();
            RaisePropertyChanged(nameof(LoggedInUser));
            IsWorking = false;
            LoginForm.FormVisible = true;
        }
        public void ShowProgressPanel(string caption)
        {
            ProgressPanelText = caption;
            IsWorking = true;
        }

        private void Pandanet_PersonalInformationUpdate(object sender, IgsUser e)
        {
            _incomingCheckboxChange = true;
            HumanLookingForGame = e.LookingForAGame;
            HumanRefusingAllGames = e.RejectsRequests;
            _incomingCheckboxChange = false;
        }
        //** USERS *************************************************************
        private bool _onlyShowLfgUsers;
        public bool OnlyShowLfgUsers
        {
            get { return _onlyShowLfgUsers; }
            set {
                SetProperty(ref _onlyShowLfgUsers, value);
                RefillChallengeableUsersFromAllUsers();
            }
        }
        public async Task RefreshUsers()
        {
            ShowProgressPanel(Localizer.IgsLoginPhase_RefreshingUsers);
            allUsers = await Connections.Igs.Commands.ListOnlinePlayersAsync();
            RefillChallengeableUsersFromAllUsers();
            IsWorking = false;
        }
        private void RefillChallengeableUsersFromAllUsers()
        {
            var filteredUsers = allUsers.Where(usr => usr.Name.ToLower().Contains(UsernameFilter.ToLower()));
            if (OnlyShowLfgUsers)
            {
                ChallengeableUsers = new ObservableCollection<IgsUser>(filteredUsers.Where(usr => usr.LookingForAGame && !usr.RejectsRequests));
            }
            else
            {
                ChallengeableUsers = new ObservableCollection<IgsUser>(filteredUsers);
            }
        }
        private List<IgsUser> allUsers = new List<IgsUser>();
        private ObservableCollection<IgsUser> _challengeableUsers = new ObservableCollection<IgsUser>();
        public ObservableCollection<IgsUser> ChallengeableUsers
        {
            get { return _challengeableUsers; }
            set { SetProperty(ref _challengeableUsers, value); }
        }
        private ObservableCollection<IgsMatchRequest> _incomingMatchRequests = new ObservableCollection<IgsMatchRequest>();
        public ObservableCollection<IgsMatchRequest> IncomingMatchRequests
        {
            get { return _incomingMatchRequests; }
            set { SetProperty(ref _incomingMatchRequests, value); }
        }


        public void SortUsers(Comparison<IgsUser> comparison)
        {
            allUsers.Sort(comparison);
            RefillChallengeableUsersFromAllUsers();
        }

        public bool ConsoleVisible
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public async Task RefreshGames()
        {
            ShowProgressPanel(Localizer.IgsLoginPhase_RefreshingGames);
            var games = await Connections.Igs.Commands.ListGamesInProgressAsync();
            ObservableGames = new ObservableCollection<IgsGameInfo>(games);
            IsWorking = false;
        }

        private ObservableCollection<IgsGameInfo> _games = new ObservableCollection<IgsGameInfo>();
        public ObservableCollection<IgsGameInfo> ObservableGames
        {
            get { return _games; }
            set { SetProperty(ref _games, value); }
        }


        public void SortGames(Comparison<IgsGameInfo> comparison)
        {
            var list = ObservableGames.ToList();
            list.Sort(comparison);
            ObservableGames = new ObservableCollection<IgsGameInfo>(list);
        }
        
        public ICommand ChallengePlayerCommand => _challengePlayerCommand ?? (_challengePlayerCommand = new MvxCommand<IgsUser>(ChallengePlayer) );

        private void ChallengePlayer(IgsUser player)
        {
            if (player == null) return;
            Mvx.RegisterSingleton<GameCreationBundle>(new IgsOutgoingChallengeBundle(player));
            OpenInNewActiveTab<GameCreationViewModel>();
        }
    }
}
