using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public abstract class LoginFormViewModel : ControlViewModelBase
    {

        
        protected readonly IGameSettings Settings;
        protected Localizer Localizer { get; }

        private bool _loginErrorMessageVisible = false;


        protected LoginFormViewModel(IGameSettings settings, Localizer localizer)
        {
            this.Settings = settings;
            this.Localizer = localizer;
            if (RememberPassword)
            {
                this._password = RetrievePassword();
            } else
            {
                this._password = "";
            }
        }


        public abstract string HyperlinkCaption { get; }
        public abstract Uri RegistrationUri {get;}

        public abstract string ServerInformation { get; }
        
        public abstract string ServerName { get; }

        public string FormCaption => Localizer.LoginFormCaption;
        public abstract string UsernameCaption { get; }
        public string PasswordCaption => Localizer.PasswordCaption;
        
        public bool LoginErrorMessageVisible
        {
            get { return _loginErrorMessageVisible; }
            set { SetProperty(ref _loginErrorMessageVisible, value); }
        }

        public string LogInButtonCaption => Localizer.LoginButtonCaption;
        public string LoginAtStartupCaption => Localizer.LoginAtStartupCaption;
        public string RememberPasswordCaption => Localizer.RememberPasswordCaption;

        private string _password;
        private string _loginErrorMessage;
        private bool _formVisible = true;
        private bool _formEnabled = true;


        protected abstract string RetrievePassword();
        protected abstract void StorePassword(string password);

        public abstract string UsernameText { get; set; }
        public string PasswordText
        {
            get { return _password; }
            set {
                SetProperty(ref _password, value);
                if (RememberPassword)
                {
                    StorePassword(value);
                }

            }
        }
        public abstract bool RememberPassword { get; set; }
        public abstract bool LoginAtStartup { get; set; }




        public string LoginErrorMessage
        {
            get { return _loginErrorMessage; }
            set { SetProperty(ref _loginErrorMessage, value); }
        }
        public bool FormVisible
        {
            get { return _formVisible; }
            set { SetProperty(ref _formVisible, value); }

        }
        public bool FormEnabled
        {
            get { return this._formEnabled; }
            set { SetProperty(ref this._formEnabled, value); }
        }




        public void LogIn()
        {
            LoginClick?.Invoke(this, new LoginEventArgs(UsernameText, PasswordText));
        }

        public event EventHandler<LoginEventArgs> LoginClick;
    }
    public class LoginEventArgs : EventArgs
    {
        public string Username { get; }
        public string Password { get; }
        public LoginEventArgs(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
    }
}
