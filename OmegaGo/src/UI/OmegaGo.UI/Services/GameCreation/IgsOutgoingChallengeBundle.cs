using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
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
        public override string TabTitle => selectedChallengeableUser.Name + " - outgoing challenge";

        public override async Task CreateChallenge(GameCreationViewModel gameCreationViewModel)
        {
            var timeSettings = gameCreationViewModel.TimeControl;
            int mainTime;
            int overtime;
            switch (timeSettings.Style)
            {
                case Core.Time.TimeControlStyle.Absolute:
                    mainTime = int.Parse(timeSettings.MainTime);
                    overtime = 0;
                    break;
                case Core.Time.TimeControlStyle.Canadian:
                    mainTime = int.Parse(timeSettings.MainTime);
                    overtime = int.Parse(timeSettings.OvertimeMinutes);
                    break;
                case Core.Time.TimeControlStyle.None:
                    mainTime = 0;
                    overtime = 0;
                    break;
                default:
                    throw new Exception("This time control system is not supported.");
            }
            await Connections.Igs.Commands.RequestBasicMatchAsync(
                selectedChallengeableUser.Name,
                gameCreationViewModel.SelectedColor,
                gameCreationViewModel.SelectedGameBoardSize.Width,
                mainTime,
                overtime
                );
            Mvx.Resolve<Notifications.IAppNotificationService>()
                .TriggerNotification(
                    new Notifications.BubbleNotification("Challenge to " + selectedChallengeableUser.Name + " sent."));
        }

        public override void OnLoad(GameCreationViewModel gameCreationViewModel)
        {
            base.OnLoad(gameCreationViewModel);
            gameCreationViewModel.FormTitle = Localizer.Creation_OutgoingIgsRequest;
            gameCreationViewModel.TimeControl.Style = Core.Time.TimeControlStyle.Canadian;
            gameCreationViewModel.TimeControl.StonesPerPeriod = "25";
            gameCreationViewModel.TimeControl.OvertimeMinutes = "10";
            gameCreationViewModel.TimeControl.MainTime = "90";
        }
    }
}
