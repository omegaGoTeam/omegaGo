using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Structures;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    public class KgsCreateChallengeBundle : KgsBundle
    {
        private readonly KgsRoom _room;
        public override bool HandicapMayBeChanged => true;
        public override bool Frozen => false;
        public override bool WillCreateChallenge => true;
        public override bool AcceptableAndRefusable => false;
        public override GameCreationFormStyle Style => GameCreationFormStyle.KgsChallengeCreation;
        public override string TabTitle => Localizer.Creation_KgsChallengeCreation;
        public override bool CanDeclineSingleOpponent => true;

        public KgsCreateChallengeBundle(KgsRoom room)
        {
            _room = room;
        }
        public override void OnLoad(GameCreationViewModel vm)
        {
            vm.FormTitle = Localizer.Creation_KgsChallengeCreation;
            base.OnLoad(vm);
        }
    }
}
