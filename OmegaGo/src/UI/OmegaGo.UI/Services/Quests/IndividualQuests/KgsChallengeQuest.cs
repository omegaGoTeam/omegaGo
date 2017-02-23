using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class KgsChallengeQuest : Quest
    {
        public KgsChallengeQuest() : base("KGS Challenge", "Finish 3 games on the KGS server.", RewardPoints.HardReward, 3)
        {
        }

        public override bool GameCompleted(GameCompletedQuestInformation info)
        {
            return info.IsPlayedByUs &&
                   info.IsOnline &&
                   info.Game is KgsGame;
        }
    }
}
