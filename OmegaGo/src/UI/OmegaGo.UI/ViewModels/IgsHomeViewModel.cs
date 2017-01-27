using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Online;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class IgsHomeViewModel : ViewModelBase
    {
        private IGameSettings _settings;

        public string LoggedInUser
        {
            get
            {
                if (Connections.Pandanet.LoggedIn)
                {
                    return Connections.Pandanet.Username;
                }
                else
                {
                    return "[not logged in]";
                }
            }
        }

        public IgsHomeViewModel(IGameSettings settings)
        {
            this._settings = settings;
            this._password = _settings.Interface.IgsPassword;
        }

        private string _progressPanelText = "Communicating with Pandanet...";
        public string ProgressPanelText
        {
            get { return _progressPanelText; }
            set { SetProperty(ref _progressPanelText, value); }

        }
        private string _loginErrorMessage = "TODO twolines or oneline";
        public string LoginErrorMessage
        {
            get { return _loginErrorMessage; }
            set { SetProperty(ref _loginErrorMessage, value); }

        }
        private bool _progressPanelVisible = false;
        public bool ProgressPanelVisible
        {
            get { return _progressPanelVisible; }
            set { SetProperty(ref _progressPanelVisible, value); }
        }
        private bool _loginScreenVisible = true;
        public bool LoginScreenVisible
        {
            get { return _loginScreenVisible; }
            set { SetProperty(ref _loginScreenVisible, value); }
        }
        private int _LoginErrorMessageOpacity = 0;
        public int LoginErrorMessageOpacity
        {
            get { return _LoginErrorMessageOpacity; }
            set { SetProperty(ref _LoginErrorMessageOpacity, value); }
        }
        public string UsernameTextBox
        {
            get { return _settings.Interface.IgsName; }
            set { _settings.Interface.IgsName = value;
                RaisePropertyChanged();
            }
        }

        private string _password;
        public string PasswordTextBox {
            get { return _password; }
            set
            {
                SetProperty(ref _password, value);
                MaybeStorePassword();
            }
        }

        public bool RememberPassword
        {
            get { return _settings.Interface.IgsRememberPassword; }
            set { _settings.Interface.IgsRememberPassword = value;
                RaisePropertyChanged();
                MaybeStorePassword();
            }
        }

        private void MaybeStorePassword()
        {
            if (_settings.Interface.IgsRememberPassword)
            {
                _settings.Interface.IgsPassword = _password;
            }
        }

        public IMvxCommand AttemptLoginCommand => new MvxCommand(async () =>
        {
            LoginErrorMessageOpacity = 0;
            ProgressPanelVisible = true;
            ProgressPanelText = "Connecting to Pandanet...";
            if (!Connections.Pandanet.ConnectionEstablished)
            {
                bool success = await Connections.Pandanet.ConnectAsync();
                if (!success)
                {
                    LoginErrorMessage = "Could not connect to Pandanet. Maybe your internet connection failed?";
                    LoginErrorMessageOpacity = 1;
                    ProgressPanelVisible = false;
                    return;
                }
            }
            ProgressPanelText = "Logging in as " + UsernameTextBox + "...";
            bool loginSuccess = await Connections.Pandanet.LoginAsync(UsernameTextBox, PasswordTextBox);
            if (loginSuccess)
            {
                RaisePropertyChanged(nameof(LoggedInUser));
                LoginScreenVisible = false;
            }
            else
            {
                LoginErrorMessage = "The username or password you entered is incorrect.";
                LoginErrorMessageOpacity = 1;
            }
            ProgressPanelVisible = false;
            //Connections.Pandanet.LoginAsync
        });

        public void Initialize()
        {
            LoginScreenVisible = !(Connections.Pandanet.LoggedIn);
            Connections.Pandanet.IncomingLine += Pandanet_IncomingLine;
            Connections.Pandanet.OutgoingLine += Pandanet_OutgoingLine;
        }

        private void Pandanet_OutgoingLine(object sender, string e)
        {
            this.Console += "\n>" + e;
        }

        public void Deinitialize()
        {
            Connections.Pandanet.IncomingLine -= Pandanet_IncomingLine;
            Connections.Pandanet.OutgoingLine -= Pandanet_OutgoingLine;
        }

        private static string consoleContents = "";
        public string Console
        {
            get { return consoleContents; }
            set
            {
                consoleContents = value;
                RaisePropertyChanged();
            }
        }

        private void Pandanet_IncomingLine(object sender, string e)
        {
            this.Console += "\n" + e;
        }

        public async void Logout()
        {
            if (!Connections.Pandanet.LoggedIn)
            {
                Debug.WriteLine("You are not yet logged in.");
                return;
            }
            ProgressPanelVisible = true;
            ProgressPanelText = "Disconnecting...";
            await Connections.Pandanet.DisconnectAsync();
            RaisePropertyChanged(nameof(LoggedInUser));
            ProgressPanelVisible = false;
            LoginScreenVisible = true;
        }
    }
}
