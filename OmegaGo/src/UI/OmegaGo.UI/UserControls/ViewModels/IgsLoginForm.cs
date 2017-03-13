using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.UserControls.ViewModels
{
    public class IgsLoginForm : LoginFormViewModel
    {

        public IgsLoginForm(Localizer localizer) : base(localizer)
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
    }
}
