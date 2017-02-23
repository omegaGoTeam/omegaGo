using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class OnlineTraditionalQuest : Quest
    {
        public OnlineTraditionalQuest() : base("Net Traditional", "Finish an online game on a 19x19 board.", RewardPoints.HardReward, 1)
        {
        }

        public override bool GameCompleted(GameCompletedQuestInformation info)
        {
            return
                info.IsPlayedByUs &&
                info.IsOnline &&
                info.Game.Info.BoardSize.Width == 19 &&
                info.Game.Info.BoardSize.Height == 19
                ;
        }
    }
}
