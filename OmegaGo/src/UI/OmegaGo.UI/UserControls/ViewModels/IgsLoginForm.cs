using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public class IgsLoginForm : LoginFormViewModel
    {

        public IgsLoginForm(Localizer localizer, IGameSettings settings) : base(settings, localizer)
        {
        }
        
        public override string HyperlinkCaption => "Create a new Pandanet account(opens a browser window)";
        public override Uri RegistrationUri => new Uri(@"http://pandanet-igs.com/igs_users/register");

        public override string ServerInformation
            =>
                "Pandanet is an online server popular in East Asia. It uses Japanese rules and Canadian time control. Pandanet is only recommended to more experienced players who wish to mainly play on 19x19 boards."
            ;
        public override string ServerName => "Pandanet - Internet Go Server";
        public override string UsernameCaption => "Pandanet username";

        protected override string RetrievePassword()
        {
            return Settings.Interface.IgsPassword;
        }

        protected override void StorePassword(string password)
        {
            Settings.Interface.IgsPassword = password;
        }

        public override string UsernameText
        {
            get { return Settings.Interface.IgsName; }
            set { Settings.Interface.IgsName = value; }
        }

        public override bool RememberPassword
        {
            get { return Settings.Interface.IgsRememberPassword; }
            set { Settings.Interface.IgsRememberPassword = value; }
        }

        public override bool LoginAtStartup
        {
            get { return Settings.Interface.IgsAutoLogin; }
            set { Settings.Interface.IgsAutoLogin = value; }
        }
    }
}
