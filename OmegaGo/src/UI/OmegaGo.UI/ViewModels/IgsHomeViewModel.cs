﻿using System;
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
            LoginForm.FormVisible = !(Connections.Igs.LoggedIn);
            Connections.Igs.IncomingMatchRequest += Pandanet_IncomingMatchRequest;
            Connections.Igs.PersonalInformationUpdate += Pandanet_PersonalInformationUpdate;
            Connections.Igs.MatchRequestAccepted += Pandanet_MatchRequestAccepted;
            if (Connections.Igs.LoggedIn)
            {
                await RefreshGames();
                await RefreshUsers();
            }
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

        public void Deinitialize()
        {
            Connections.Igs.IncomingMatchRequest -= Pandanet_IncomingMatchRequest;
            Connections.Igs.MatchRequestAccepted -= Pandanet_MatchRequestAccepted;
            Connections.Igs.PersonalInformationUpdate -= Pandanet_PersonalInformationUpdate;
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
            await Connections.Igs.ToggleAsync("open", !value);
            ProgressPanelVisible = false;
        }
        private async void ToggleHumanLookingForGameTo(bool value)
        {
            ShowProgressPanel("Toggling whether we are looking for games...");
            await Connections.Igs.ToggleAsync("looking", value);
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
                await RefreshGames();
                await RefreshUsers();
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
                Debug.WriteLine("You are not yet logged in.");
                return;
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
            allUsers = await Connections.Igs.ListOnlinePlayersAsync();
            RefillChallengeableUsersFromAllUsers();
            ProgressPanelVisible = false;
        }
        private void RefillChallengeableUsersFromAllUsers()
        {
            if (OnlyShowLfgUsers)
            {
                ChallengeableUsers = new ObservableCollection<IgsUser>(allUsers.Where(usr => usr.LookingForAGame));
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
            ChallengeableUsers = new ObservableCollection<IgsUser>(allUsers);
        }
        //** GAMES *************************************************************



        public async Task RefreshGames()
        {
            ShowProgressPanel("Refreshing the list of observable games...");
            var games = await Connections.Igs.ListGamesInProgressAsync();
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
            set { SetProperty(ref _selectedSpectatableGame, value); }
        }
        private IgsUser _selectedChallengeableUser;
        public IgsUser SelectedChallengeableUser
        {
            get { return _selectedChallengeableUser; }
            set { SetProperty(ref _selectedChallengeableUser, value); }
        }

        public IMvxCommand ChallengeSelectedPlayer => new MvxCommand(() =>
        {
            if (SelectedChallengeableUser != null)
            {
                Mvx.RegisterSingleton<GameCreationBundle>(new IgsChallengeBundle(SelectedChallengeableUser));
                ShowViewModel<GameCreationViewModel>();
            }
        });
        public IMvxCommand ObserveSelectedGame => new MvxCommand(async () =>
        {
            ShowProgressPanel("Initiating observation of a game...");
            var onlinegame = await Connections.Igs.StartObserving(SelectedSpectatableGame);
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
        });

        public void StartGame(IgsGame game)
        {
            Mvx.RegisterSingleton<IGame>(game);
            ShowViewModel<OnlineGameViewModel>();
        }
    }
}
