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


        public virtual string HyperlinkCaption => "Create a new Pandanet account(opens a browser window)";
        public virtual Uri RegistrationUri => new Uri(@"http://pandanet-igs.com/igs_users/register");

        public virtual string ServerInformation
            =>
                "Pandanet is an online server popular in East Asia. It uses Japanese rules and Canadian time control. Pandanet is only recommended to more experienced players who wish to mainly play on 19x19 boards."
            ;
        public virtual string ServerName => "Pandanet - Internet Go Server";

        public virtual string FormCaption => "Login";
        public virtual string UsernameCaption => "Pandanet username";
        public virtual string PasswordCaption => "Password";

        public double LoginErrorMessageOpacity => 1;
        public string LogInButtonCaption => "Log In";
        public string LoginAtStartupCaption => "Log in whenever you start omegaGo";
        public string RememberPasswordCaption => "Remember password";

        // TODO bind to events
        public string UsernameText { get; set; } 
        public string PasswordText { get; set; } 
        public bool RememberPassword { get; set; }
        public bool LoginAtStartup { get; set; }
        public string LoginErrorMessage { get; set; }

    }
}
