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
    class IgsOutgoingChallengeBundle : IgsBundle
    {
        private IgsUser selectedChallengeableUser;

        public IgsOutgoingChallengeBundle(IgsUser selectedChallengeableUser)
        {
            this.selectedChallengeableUser = selectedChallengeableUser;
        }

        public override string OpponentName => selectedChallengeableUser.Name;
        public override GameCreationFormStyle Style => GameCreationFormStyle.OutgoingIgs;
        public override bool AcceptableAndRefusable => false;
        public override bool WillCreateChallenge => true;
        public override bool Frozen => false;

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
            base.OnLoad(gameCreationViewModel);
            gameCreationViewModel.FormTitle = Localizer.Creation_OutgoingIgsRequest;
        }
    }
}
