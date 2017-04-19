using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class UnevenStrengthQuest : Quest
    {
        public UnevenStrengthQuest() : base("Uneven Strength", "Finish two solo or online games where one player has a handicap of two stones or more.", RewardPoints.MediumReward, 2)
        {
        }

        public override bool GameCompleted(GameCompletedQuestInformation info)
        {
            return
                info.IsPlayedByUs &&
                !info.IsHotseat &&
                info.Game.Info.NumberOfHandicapStones >= 2
                ;
        }

        // No "Try This Now" because we don't know if the user wants a local or online game.
        public override bool TryThisNowButtonVisible => false;
    }
}
