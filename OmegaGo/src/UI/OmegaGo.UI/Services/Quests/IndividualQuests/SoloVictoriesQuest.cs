using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class SoloVictoriesQuest : Quest
    {
        public SoloVictoriesQuest() : base("Human > Computer", "Win 3 solo games against an AI program.", RewardPoints.MediumReward, 3)
        {
        }
        public override Type GetViewModelToTry()
        {
            Mvx.RegisterSingleton<GameCreation.GameCreationBundle>(new SoloBundle());
            return typeof(GameCreationViewModel);
        }

        public override bool GameCompleted(GameCompletedQuestInformation info)
        {
            return
                info.IsPlayedByUs &&
                info.IsVictory &&
                !info.IsHotseat
                ;
        }
        public override bool TryThisNowButtonVisible => true;
    }
}
