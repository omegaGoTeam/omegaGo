using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Structures;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.GameCreationBundle;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class IgsHomeViewModel : ViewModelBase
    {
        private readonly IGameSettings _settings;
       

        public IgsHomeViewModel(IGameSettings settings)
        {
            this._settings = settings;
            this.LoginForm = new IgsLoginForm(this.Localizer, _settings);
            LoginForm.LoginClick +=  LoginForm_LoginClick;

        }

        private async void LoginForm_LoginClick(object sender, LoginEventArgs e)
        {
            await this.AttemptLoginCommand(e.Username, e.Password);
        }

        public async Task Initialize()
        {
            LoginForm.FormVisible = Connections.Igs.Composure != IgsComposure.Ok; 
            Connections.Igs.Events.IncomingMatchRequest += Pandanet_IncomingMatchRequest;
            Connections.Igs.Events.PersonalInformationUpdate += Pandanet_PersonalInformationUpdate;
            Connections.Igs.Events.MatchRequestAccepted += Pandanet_MatchRequestAccepted;
            Connections.Igs.Events.Disconnected += Events_Disconnected;
            Connections.Igs.Events.LoginPhaseChanged += Events_LoginPhaseChanged;
            if (LoginForm.FormVisible && Connections.Igs.Composure != IgsComposure.Disconnected)
            {
                LoginForm.FormEnabled = false;
                LoginForm.LoginErrorMessage = "Login already in progress...";
                LoginForm.LoginErrorMessageOpacity = 1;
                Connections.Igs.Events.LoginComplete += OnLoginComplete;
            }
            else if (Connections.Igs.LoggedIn)
            {
                await EnterIgsLobbyLoggedIn();
            }
        }

        private void Events_LoginPhaseChanged(object sender, IgsLoginPhase e)
        {
            this.LoginForm.LoginErrorMessage = Localizer.GetString("IgsLoginPhase_" + e.ToString()); ;
        }

        private void Events_Disconnected(object sender, EventArgs e)
        {
            this.LoginForm.FormEnabled = true;
            this.LoginForm.FormVisible = true;
            this.LoginForm.LoginErrorMessage = "You have been disconnected.";
            this.LoginForm.LoginErrorMessageOpacity = 1;

        }

        private async void OnLoginComplete(object sender, bool success)
        {
            if (success)
            {
                await EnterIgsLobbyLoggedIn();
            }
            else
            {
                this.LoginForm.FormEnabled = true;
                this.LoginForm.LoginErrorMessage = "Login failed.";
                this.LoginForm.LoginErrorMessageOpacity = 1;
            }
        }

        private async Task EnterIgsLobbyLoggedIn()
        {
            allUsers = Connections.Igs.Data.OnlineUsers;
            ObservableGames = new ObservableCollection<IgsGameInfo>(Connections.Igs.Data.GamesInProgress);
            RefillChallengeableUsersFromAllUsers();
            LoginForm.FormVisible = false;
            LoginForm.FormEnabled = true;
            LoginForm.LoginErrorMessageOpacity = 0;
            await Connections.Igs.Commands.RequestPersonalInformationUpdate(Connections.Igs.Username);
        }

        public void Deinitialize()
        {
            Connections.Igs.Events.IncomingMatchRequest -= Pandanet_IncomingMatchRequest;
            Connections.Igs.Events.MatchRequestAccepted -= Pandanet_MatchRequestAccepted;
            Connections.Igs.Events.PersonalInformationUpdate -= Pandanet_PersonalInformationUpdate;
            Connections.Igs.Events.LoginComplete -= OnLoginComplete;
            Connections.Igs.Events.Disconnected -= Events_Disconnected;
            Connections.Igs.Events.LoginPhaseChanged -= Events_LoginPhaseChanged;
        }

        public LoginFormViewModel LoginForm { get; }
    

        private void Pandanet_MatchRequestAccepted(object sender, IgsGame e)
        {
            StartGame(e);
        }

        private void Pandanet_IncomingMatchRequest(IgsMatchRequest obj)
        {
            this.IncomingMatchRequests.Add(obj);
        }

     

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
                else
                {
                    return "[not logged in]";
                }
            }
        }


        private bool _incomingCheckboxChange = false;
        private string _progressPanelText = "Communicating with Pandanet...";

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
            ShowProgressPanel("Toggling whether we are open...");
            await Connections.Igs.Commands.ToggleAsync("open", !value);
            ProgressPanelVisible = false;
        }
        private async void ToggleHumanLookingForGameTo(bool value)
        {
            ShowProgressPanel("Toggling whether we are looking for games...");
            await Connections.Igs.Commands.ToggleAsync("looking", value);
            ProgressPanelVisible = false;
        }

        public string ProgressPanelText
        {
            get { return _progressPanelText; }
            set { SetProperty(ref _progressPanelText, value); }

        }
        private bool _progressPanelVisible = false;
        public bool ProgressPanelVisible
        {
            get { return _progressPanelVisible; }
            set { SetProperty(ref _progressPanelVisible, value); }
        }
        public async Task AttemptLoginCommand(string username, string password)
        {
            LoginForm.LoginErrorMessageOpacity = 1;
            ProgressPanelVisible = true;
            ProgressPanelText = "Connecting to Pandanet...";
            LoginForm.FormEnabled = false;
            LoginForm.LoginErrorMessage = "Connecting to Pandanet...";
            if (!Connections.Igs.ConnectionEstablished)
            {
                bool success = await Connections.Igs.ConnectAsync();
                if (!success)
                {
                    LoginForm.LoginErrorMessage = "Could not connect to Pandanet. Maybe your internet connection failed?";
                    LoginForm.LoginErrorMessageOpacity = 1;
                    ProgressPanelVisible = false;
                    return;
                }
            }


            ProgressPanelText = "Logging in as " + username + "...";
            LoginForm.LoginErrorMessage = "Logging in as " + username + "...";
            bool loginSuccess = await Connections.Igs.LoginAsync(username, password);
            LoginForm.FormEnabled = true;
            if (loginSuccess)
            {
                RaisePropertyChanged(nameof(LoggedInUser));
                LoginForm.FormVisible = false;
                LoginForm.LoginErrorMessageOpacity = 0;
                await EnterIgsLobbyLoggedIn();
            }
            else
            {
                LoginForm.LoginErrorMessage = "The username or password you entered is incorrect.";
                LoginForm.LoginErrorMessageOpacity = 1;
            }
            ProgressPanelVisible = false;
        }

        public async void Logout()
        {
            if (!Connections.Igs.LoggedIn)
            {
                Debug.WriteLine("You are not logged in.");
            }
            ProgressPanelVisible = true;
            ProgressPanelText = "Disconnecting...";
            await Connections.Igs.DisconnectAsync();
            RaisePropertyChanged(nameof(LoggedInUser));
            ProgressPanelVisible = false;
            LoginForm.FormVisible = true;
        }
        public void ShowProgressPanel(string caption)
        {
            ProgressPanelText = caption;
            ProgressPanelVisible = true;
        }
        private void Pandanet_PersonalInformationUpdate(object sender, IgsUser e)
        {
            _incomingCheckboxChange = true;
            this.HumanLookingForGame = e.LookingForAGame;
            this.HumanRefusingAllGames = e.RejectsRequests;
            _incomingCheckboxChange = false;
        }
        //** USERS *************************************************************
        private bool _onlyShowLfgUsers = false;
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
            ShowProgressPanel("Refreshing the list of logged-in users...");
            allUsers = await Connections.Igs.Commands.ListOnlinePlayersAsync();
            RefillChallengeableUsersFromAllUsers();
            ProgressPanelVisible = false;
        }
        private void RefillChallengeableUsersFromAllUsers()
        {
            if (OnlyShowLfgUsers)
            {
                ChallengeableUsers = new ObservableCollection<IgsUser>(allUsers.Where(usr => usr.LookingForAGame && !usr.RejectsRequests));
            }
            else
            {
                ChallengeableUsers = new ObservableCollection<IgsUser>(allUsers);
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
            ShowProgressPanel("Refreshing the list of observable games...");
            var games = await Connections.Igs.Commands.ListGamesInProgressAsync();
            ObservableGames = new ObservableCollection<IgsGameInfo>(games);
            ProgressPanelVisible = false;
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

        private IgsGameInfo _selectedSpectatableGame;
        public IgsGameInfo SelectedSpectatableGame
        {
            get { return _selectedSpectatableGame; }
            set {
                SetProperty(ref _selectedSpectatableGame, value);
                ObserveSelectedGame.RaiseCanExecuteChanged();
            }
        }
        private IgsUser _selectedChallengeableUser;
        public IgsUser SelectedChallengeableUser
        {
            get { return _selectedChallengeableUser; }
            set {
                SetProperty(ref _selectedChallengeableUser, value);
                ChallengeSelectedPlayer.RaiseCanExecuteChanged();
            }
        }

        private IMvxCommand _challengedSelectedPlayer;
        public IMvxCommand ChallengeSelectedPlayer => _challengedSelectedPlayer ?? (_challengedSelectedPlayer = new MvxCommand(() =>
        {
            if (SelectedChallengeableUser != null)
            {
                Mvx.RegisterSingleton<GameCreationBundle>(new IgsChallengeBundle(SelectedChallengeableUser));
                ShowViewModel<GameCreationViewModel>();
            }
        }, ()=>SelectedChallengeableUser !=null && !SelectedChallengeableUser.RejectsRequests));
        private IMvxCommand _observeSelectedGame;
        public IMvxCommand ObserveSelectedGame => _observeSelectedGame ?? (_observeSelectedGame = new MvxCommand(async () =>
        {
            ShowProgressPanel("Initiating observation of a game...");
            var onlinegame = await Connections.Igs.Commands.StartObserving(SelectedSpectatableGame);
            if (onlinegame == null)
            {
                // TODO Petr: error report
            }
            else
            {
                Mvx.RegisterSingleton<IGame>(onlinegame); 
                ShowViewModel<ObserverGameViewModel>();
            }
            ProgressPanelVisible = false;
        }, ()=> SelectedSpectatableGame != null));

        public void StartGame(IgsGame game)
        {
            Mvx.RegisterSingleton<IGame>(game);
            ShowViewModel<OnlineGameViewModel>();
        }
    }
}
