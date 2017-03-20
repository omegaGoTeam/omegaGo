using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.UserControls.ViewModels
{
    class KgsLoginForm : LoginFormViewModel
    {
        public KgsLoginForm(Localizer localizer, IGameSettings settings) : base(settings, localizer)
        {
        }

        public override string HyperlinkCaption => Localizer.KgsHyperlink;

        public override Uri RegistrationUri => new Uri("http://files.gokgs.com/javaBin/cgoban.exe");

        public override string ServerInformation => Localizer.KgsServerInfo;
        public override string ServerName => Localizer.KgsServerCaption;

        public override string UsernameCaption => Localizer.KgsUsernameCaption;
        protected override string RetrievePassword()
        {
            return Settings.Interface.KgsPassword;
        }

        protected override void StorePassword(string password)
        {
            Settings.Interface.KgsPassword = password;
        }

        public override string UsernameText
        {
            get { return Settings.Interface.KgsName; }
            set { Settings.Interface.KgsName = value; }
        }

        public override bool RememberPassword
        {
            get { return Settings.Interface.KgsRememberPassword; }
            set { Settings.Interface.KgsRememberPassword = value; }
        }

        public override bool LoginAtStartup
        {
            get { return Settings.Interface.KgsAutoLogin; }
            set { Settings.Interface.KgsAutoLogin = value; }
        }
    }
}
