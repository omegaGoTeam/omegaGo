using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public abstract class LoginFormViewModel : ControlViewModelBase
    {
        protected Localizer Localizer { get; }

        protected LoginFormViewModel(Localizer localizer)
        {
            this.Localizer = localizer;
        }

        
        public abstract string HyperlinkCaption { get; }
        public abstract Uri RegistrationUri {get;}

        public abstract string ServerInformation { get; }
        
        public abstract string ServerName { get; }
        
        public string FormCaption => "Login";
        public abstract string UsernameCaption { get; }
        public string PasswordCaption => "Password";

        public double LoginErrorMessageOpacity => 1;
        public string LogInButtonCaption => "Log In";
        public string LoginAtStartupCaption => "Log in whenever you start omegaGo";
        public string RememberPasswordCaption => "Remember password";
        
        private string _username;
        private string _password;
        private string _loginErrorMessage;
        private bool _rememberPassword;
        private bool _loginAtStartup;
        private bool _formVisible = true;
        private bool _formDisabled;
        public bool FormVisible
        {
            get { return _formVisible; }
            set { SetProperty(ref _formVisible, value); }

        }
        public bool FormDisabled
        {
            get { return _formDisabled; }
            set { SetProperty(ref _formDisabled, value); }

        }
        public string UsernameText {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }
        public string PasswordText
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }
        public bool RememberPassword
        {
            get { return _rememberPassword; }
            set { SetProperty(ref _rememberPassword, value); }
        }
        public bool LoginAtStartup
        {
            get { return _loginAtStartup; }
            set { SetProperty(ref _loginAtStartup, value); }
        }
        public string LoginErrorMessage
        {
            get { return _loginErrorMessage; }
            set { SetProperty(ref _loginErrorMessage, value); }
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
