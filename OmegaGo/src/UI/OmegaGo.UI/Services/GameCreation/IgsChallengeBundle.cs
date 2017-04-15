using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Igs;
using OmegaGo.UI.Services.Online;
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

        public override string OpponentName => selectedChallengeableUser.Name;
        public override GameCreationFormStyle Style => GameCreationFormStyle.OutgoingIgs;
        public override bool SupportsChangingRulesets => false;
        public override bool SupportsRectangularBoards => false;
        public override bool AcceptableAndRefusable => false;
        public override bool Playable => false;
        public override bool BlackAndWhiteVisible => false;
        public override bool WillCreateChallenge => true;
        public override bool KomiIsAvailable => false;
        public override bool HandicapMayBeChanged => false;
        public override bool IsIgs => true;

        public override async Task CreateChallenge(GameCreationViewModel gameCreationViewModel)
        {
            await Connections.Igs.Commands.RequestBasicMatchAsync(
                selectedChallengeableUser.Name,
                gameCreationViewModel.SelectedColor,
                gameCreationViewModel.SelectedGameBoardSize.Width,
                int.Parse(gameCreationViewModel.TimeControl.MainTime),
                int.Parse(gameCreationViewModel.TimeControl.OvertimeMinutes)
                ); 
            // TODO Petr: make use correct things for different time controls
        }

        public override void OnLoad(GameCreationViewModel gameCreationViewModel)
        {
            gameCreationViewModel.FormTitle = Localizer.Creation_OutgoingIgsRequest;
            gameCreationViewModel.IgsLimitation = true;
            gameCreationViewModel.Server = "IGS";
            gameCreationViewModel.SelectedRuleset = Core.Rules.RulesetType.Japanese;
        }
    }
}
