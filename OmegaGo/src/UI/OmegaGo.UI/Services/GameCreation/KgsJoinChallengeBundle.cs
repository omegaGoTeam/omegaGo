using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    public class KgsJoinChallengeBundle : KgsBundle
    {
        private readonly KgsChallenge _challenge;

        public override bool HandicapMayBeChanged => false;
        public override bool Frozen => true;
        public override bool WillCreateChallenge => false;
        public override bool AcceptableAndRefusable => true;
        public override string TabTitle => _challenge.ToString();
        public override GameCreationFormStyle Style => GameCreationFormStyle.KgsChallengeNegotiation;
        public override bool CanDeclineSingleOpponent => false;

        public KgsJoinChallengeBundle(KgsChallenge challenge)
        {
            _challenge = challenge;
        }
        public override void OnLoad(GameCreationViewModel vm)
        {
            vm.FormTitle = Localizer.Creationg_KgsChallenge;
            vm.RefusalCaption = Localizer.UnjoinChallenge;
            base.OnLoad(vm);
        }
    }
}
