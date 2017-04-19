using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class IgsChallengeQuest : Quest
    {
        public IgsChallengeQuest() : base("Pandanet Challenge", "Finish 3 games on the Pandanet server.", RewardPoints.HardReward, 3)
        {
        }

        public override Type GetViewModelToTry()
        {
            return typeof(IgsHomeViewModel);
        }

        public override bool GameCompleted(GameCompletedQuestInformation info)
        {
            return info.IsPlayedByUs &&
                   info.IsOnline &&
                   info.Game is IgsGame;
        }
        public override bool TryThisNowButtonVisible => true;
    }
}
