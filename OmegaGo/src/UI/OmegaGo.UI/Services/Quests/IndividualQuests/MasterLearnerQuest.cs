using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class MasterLearnerQuest : Quest
    {
        public MasterLearnerQuest()
            : base("Master Learner", "Solve 5 tsumego problems without making a false move.", RewardPoints.ExtremeReward, 5)
        {
        }

        // TODO not yet implemented   
        public override Type GetViewModelToTry()
        {
            return typeof(TsumegoMenuViewModel);
        }

        public override bool TryThisNowButtonVisible => true;
    }
}
