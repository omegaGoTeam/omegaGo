using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.UserControls.ViewModels
{
    class KgsLoginForm : LoginFormViewModel
    {
        public KgsLoginForm(Localizer localizer) : base(localizer)
        {
        }

        public override string HyperlinkCaption => "Download the official KGS client";

        public override Uri RegistrationUri => new Uri("http://files.gokgs.com/javaBin/cgoban.exe");

        public override string ServerInformation
            =>
                "You first need to create an account using the official KGS client. Download it on a desktop computer, login as a guest and then click User->Register."
            ;

        public override string ServerName => "KGS Go Server";

        public override string UsernameCaption => "KGS Username";
    }
}
