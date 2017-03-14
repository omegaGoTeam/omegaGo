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
        
        public string FormCaption => "Login";
        public abstract string UsernameCaption { get; }
        public string PasswordCaption => "Password";

        private double _loginErrorMessageOpacity = 0;
        public double LoginErrorMessageOpacity
        {
            get { return _loginErrorMessageOpacity; }
            set { SetProperty(ref _loginErrorMessageOpacity, value); }
        }
        public string LogInButtonCaption => "Log In";
        public string LoginAtStartupCaption => "Log in whenever you start omegaGo";
        public string RememberPasswordCaption => "Remember password";

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
