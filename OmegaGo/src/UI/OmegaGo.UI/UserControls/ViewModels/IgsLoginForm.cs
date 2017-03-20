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

        public override string HyperlinkCaption => Localizer.IgsHyperlink;
        public override Uri RegistrationUri => new Uri(@"http://pandanet-igs.com/igs_users/register");

        public override string ServerInformation
            => Localizer.IgsServerInfo;

        public override string ServerName => Localizer.IgsServerCaption;
        public override string UsernameCaption => Localizer.IgsUsernameCaption;

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
