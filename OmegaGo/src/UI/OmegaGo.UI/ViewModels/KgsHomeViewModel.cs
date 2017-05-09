using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

        private ObservableCollection<KgsRoom> _allRooms = new ObservableCollection<KgsRoom>();

        private ObservableCollection<KgsGameContainer> _gameContainers = new ObservableCollection<KgsGameContainer>();

        private IMvxCommand _joinRoomCommand;
        private IMvxCommand _createChallengeCommand;

        private IMvxCommand _joinSelectedGameChannelCommand;

        private KgsGameContainer _selectedGameContainer;
        private KgsGameChannel _selectedGameChannel;

        private KgsRoom _selectedRoom;

        private bool _showRobots = true;
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
                // TODO Petr: figure out a way to inform the UI when unjoin/join happens
                UpdateBindings();
            }
        }, () => this.SelectedRoom != null && this.SelectedRoom.Joined));

        public IMvxCommand JoinRoomCommand => _joinRoomCommand ?? (_joinRoomCommand = new MvxCommand(async () =>
        {
            if (this.SelectedRoom != null && !this.SelectedRoom.Joined)
            {
                await Connections.Kgs.Commands.JoinRoomAsync(this.SelectedRoom);
                UpdateBindings();
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
        public IMvxCommand JoinSelectedGameChannelCommand
            => _joinSelectedGameChannelCommand ?? (_joinSelectedGameChannelCommand = new MvxCommand(async () =>
            {
                if (this.SelectedGameChannel != null)
                {
                    if (this.SelectedGameChannel is KgsTrueGameChannel)
                    {
                        var trueChannel = this.SelectedGameChannel as KgsTrueGameChannel;
                        await Connections.Kgs.Commands.ObserveGameAsync(trueChannel.GameInfo);
                    }
                    else if (this.SelectedGameChannel is KgsChallenge)
                    {
                        var challenge = this.SelectedGameChannel as KgsChallenge;
                        await Connections.Kgs.Commands.JoinAndSubmitSelfToChallengeAsync(challenge);
                        Mvx.RegisterSingleton<GameCreationBundle>(new KgsJoinChallengeBundle(challenge));
                        OpenInNewActiveTab<GameCreationViewModel>();
                    }
                }
            }, () => this.SelectedGameChannel != null));

        public IMvxCommand LogoutCommand
            => new MvxCommand(async () => { await Connections.Kgs.Commands.LogoutAsync(); });

        public IMvxCommand RefreshControlsCommand => new MvxCommand(UpdateBindings);

        public ObservableCollection<KgsGameContainer> GameContainers => Connections.Kgs.Data.GameContainers;

        public ObservableCollection<KgsRoom> AllRooms => Connections.Kgs.Data.AllRooms;

        public ObservableCollection<KgsGameChannel> SelectedGameContainerChannels
        {
            get
            {
                if (this.SelectedGameContainer == null)
                {
                    return new ObservableCollection<KgsGameChannel>();
                }
                else
                {
                    return this.SelectedGameContainer.AllChannelsCollection;
                }
            }
        }

        public KgsGameContainer SelectedGameContainer
        {
            get { return _selectedGameContainer; }
            set
            {
                SetProperty(ref _selectedGameContainer, value);
                RaisePropertyChanged(nameof(KgsHomeViewModel.SelectedGameContainerChannels));
            }
        }

        public KgsGameChannel SelectedGameChannel
        {
            get { return _selectedGameChannel; }
            set
            {
                SetProperty(ref _selectedGameChannel, value);
                this.JoinSelectedGameChannelCommand.RaiseCanExecuteChanged();
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
            this.JoinSelectedGameChannelCommand.RaiseCanExecuteChanged();
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

            this.LoginForm.LoginErrorMessage = "Logging in as " + username + "...";
            await Connections.Kgs.LoginAsync(username, password);
        }

        private void Events_Disconnection(object sender, string e)
        {
            this.LoginForm.FormVisible = true;
            this.LoginForm.FormEnabled = true;
            this.LoginForm.LoginErrorMessage = "You have been disconnected.";
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
        }

        private async void LoginForm_LoginClick(object sender, LoginEventArgs e)
        {
            await AttemptLoginCommand(e.Username, e.Password);
        }
    }
}