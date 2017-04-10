using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.GameCreation
{
    class IgsChallengeBundle : GameCreationBundle.GameCreationBundle
    {
        private IgsUser selectedChallengeableUser;

        public IgsChallengeBundle(IgsUser selectedChallengeableUser)
        {
            this.selectedChallengeableUser = selectedChallengeableUser;
        }

        public override GameCreationFormStyle Style => GameCreationFormStyle.OutgoingIgs;
        public override bool SupportsRectangularBoards => false;

        public override void OnLoad(GameCreationViewModel gameCreationViewModel)
        {
            gameCreationViewModel.FormTitle = Localizer.Creation_OutgoingIgsRequest;
            gameCreationViewModel.IgsLimitation = true;
            gameCreationViewModel.Server = "IGS";
            gameCreationViewModel.SelectedRuleset = Core.Rules.RulesetType.Japanese;
        }
    }
}
