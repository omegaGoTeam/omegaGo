using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.ViewModels
{
    public class KgsHomeViewModel : ViewModelBase
    {
        private readonly IGameSettings _settings;

        private IMvxCommand _joinRoomCommand;
        private IMvxCommand _createChallengeCommand;

        private IMvxCommand _joinChannelCommand;

        private KgsGameContainer _selectedGameContainer;
        private KgsGameChannel _selectedGameChannel;

        private KgsRoom _selectedRoom;

        private IMvxCommand _unjoinRoomCommand;


        public KgsHomeViewModel(IGameSettings settings)
        {
            _settings = settings;
            this.LoginForm = new KgsLoginForm(this.Localizer, _settings);
            this.LoginForm.LoginClick += LoginForm_LoginClick;
        }

        public IMvxCommand UnjoinRoomCommand => _unjoinRoomCommand ?? (_unjoinRoomCommand = new MvxCommand(async () =>
        {
            if (this.SelectedRoom != null && this.SelectedRoom.Joined)
            {
                await Connections.Kgs.Commands.UnjoinRoomAsync(this.SelectedRoom);
            }
        }, () => this.SelectedRoom != null && this.SelectedRoom.Joined));

        public IMvxCommand JoinRoomCommand => _joinRoomCommand ?? (_joinRoomCommand = new MvxCommand(async () =>
        {
            if (this.SelectedRoom != null && !this.SelectedRoom.Joined)
            {
                await Connections.Kgs.Commands.JoinRoomAsync(this.SelectedRoom);
            }
        }, () => this.SelectedRoom != null && !this.SelectedRoom.Joined));
        public IMvxCommand CreateChallengeCommand => _createChallengeCommand ?? (_createChallengeCommand = new MvxCommand(() =>
        {
            if (this.SelectedRoom != null && this.SelectedRoom.Joined)
            {
                Mvx.RegisterSingleton<GameCreationBundle>(new KgsCreateChallengeBundle(this.SelectedRoom));
                OpenInNewActiveTab<GameCreationViewModel>();
            }
        }, () => this.SelectedRoom != null && this.SelectedRoom.Joined));
        public ICommand JoinChannelCommand
            => _joinChannelCommand ?? (_joinChannelCommand = new MvxAsyncCommand<KgsChannel>(JoinChannelAsync));

        private async Task JoinChannelAsync(KgsChannel channel)
        {
            if (channel is KgsTrueGameChannel)
            {
                var trueChannel = channel as KgsTrueGameChannel;
                await Connections.Kgs.Commands.ObserveGameAsync(trueChannel.GameInfo);
            }
            else if (channel is KgsChallenge)
            {
                var challenge = channel as KgsChallenge;
                await Connections.Kgs.Commands.JoinAndSubmitSelfToChallengeAsync(challenge);
            }
        }
        
        public IMvxCommand LogoutCommand
            => new MvxCommand(async () => { await Connections.Kgs.Commands.LogoutAsync(); });

        public IMvxCommand RefreshControlsCommand => new MvxCommand(UpdateBindings);

        public ObservableCollection<KgsGameContainer> GameContainers => Connections.Kgs.Data.GameContainers;

        public ObservableCollection<KgsRoom> AllRooms => Connections.Kgs.Data.AllRooms;

        public string LoggedInUser
        {
            get
            {
                if (Connections.Kgs.LoggedIn)
                {
                    return Connections.Kgs.Username;
                }
                return Localizer.NotLoggedIn;
            }
        }

        public ObservableCollection<KgsChallenge> SelectedGameContainerChallenges
        {
            get
            {
                if (this.SelectedGameContainer == null)
                {
                    return new ObservableCollection<KgsChallenge>();
                }
                else
                {
                    return this.SelectedGameContainer.Challenges;
                }
            }
        }

        public ObservableCollection<KgsTrueGameChannel> SelectedGameContainerGames
        {
            get
            {
                if (this.SelectedGameContainer == null)
                {
                    return new ObservableCollection<KgsTrueGameChannel>();
                }
                else
                {
                    return this.SelectedGameContainer.Games;
                }
            }
        }

        public KgsGameContainer SelectedGameContainer
        {
            get { return _selectedGameContainer; }
            set
            {
                SetProperty(ref _selectedGameContainer, value);
                RaisePropertyChanged(nameof(KgsHomeViewModel.SelectedGameContainerChallenges));
                RaisePropertyChanged(nameof(KgsHomeViewModel.SelectedGameContainerGames));
            }
        }

        public KgsGameChannel SelectedGameChannel
        {
            get { return _selectedGameChannel; }
            set
            {
                SetProperty(ref _selectedGameChannel, value);
            }
        }

        public KgsRoom SelectedRoom
        {
            get { return _selectedRoom; }
            set
            {
                SetProperty(ref _selectedRoom, value);
                MinorBindingsUpdate();
            }
        }

        private void MinorBindingsUpdate()
        {
            this.JoinRoomCommand.RaiseCanExecuteChanged();
            this.UnjoinRoomCommand.RaiseCanExecuteChanged();
            this.CreateChallengeCommand.RaiseCanExecuteChanged();
        }

        public LoginFormViewModel LoginForm { get; }

        public void Init()
        {
            Connections.Kgs.Events.LoginPhaseChanged += Events_LoginPhaseChanged;
            Connections.Kgs.Events.Disconnection += Events_Disconnection;
            Connections.Kgs.Data.SomethingChanged += MinorBindingsUpdate;
            Connections.Kgs.Events.LoginComplete += Events_LoginComplete;

            if (Connections.Kgs.LoggedIn)
            {
                this.LoginForm.FormVisible = false;
                UpdateBindings();
            }
            else
            {
                this.LoginForm.FormVisible = true;
                if (Connections.Kgs.LoggingIn)
                {
                    this.LoginForm.FormEnabled = false;
                    this.LoginForm.LoginErrorMessage = Localizer.Igs_LoginAlreadyInProgress;
                    this.LoginForm.LoginErrorMessageVisible = true;

                }
                else
                {
                    this.LoginForm.LoginErrorMessage = "";
                    this.LoginForm.LoginErrorMessageVisible = false;
                    this.LoginForm.FormEnabled = true;
                }
            }
        }

        private void Events_LoginComplete(object sender, bool success)
        {
            this.LoginForm.FormEnabled = true;
            if (!success)
            {

                this.LoginForm.LoginErrorMessage = "The username or password you entered is incorrect.";
                this.LoginForm.LoginErrorMessageVisible = true;
            }
        }

        public override Task<bool> CanCloseViewModelAsync()
        {
            Connections.Kgs.Events.LoginPhaseChanged -= Events_LoginPhaseChanged;
            Connections.Kgs.Events.Disconnection -= Events_Disconnection;
            Connections.Kgs.Data.SomethingChanged -= MinorBindingsUpdate;
            Connections.Kgs.Events.LoginComplete -= Events_LoginComplete;
            return base.CanCloseViewModelAsync();
        }

        public async Task AttemptLoginCommand(string username, string password)
        {
            this.LoginForm.LoginErrorMessageVisible = true;
            this.LoginForm.FormEnabled = false;

            await Connections.Kgs.LoginAsync(username, password);
        }

        private void Events_Disconnection(object sender, string e)
        {
            this.LoginForm.FormVisible = true;
            this.LoginForm.FormEnabled = true;
            this.LoginForm.LoginErrorMessage = Localizer.YouHaveBeenDisconnected;
        }


        private void Events_LoginPhaseChanged(object sender, KgsLoginPhase e)
        {
            this.LoginForm.LoginErrorMessage = Localizer.GetString("KgsLoginPhase_" + e.ToString());
            UpdateBindings();
            if (e == KgsLoginPhase.Done)
            {
                this.LoginForm.FormVisible = false;
                this.LoginForm.LoginErrorMessageVisible = false;
            }
        }

        private void UpdateBindings()
        {
            RaisePropertyChanged(nameof(AllRooms));
            RaisePropertyChanged(nameof(GameContainers));
            if (SelectedGameContainer == null)
            {
                SelectedGameContainer = GameContainers.FirstOrDefault();
            }
            RaisePropertyChanged(nameof(LoggedInUser));
        }

        private async void LoginForm_LoginClick(object sender, LoginEventArgs e)
        {
            await AttemptLoginCommand(e.Username, e.Password);
        }
    }
}