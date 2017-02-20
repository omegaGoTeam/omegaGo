using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class GreatLearnerQuest : Quest
    {
        public GreatLearnerQuest() : base("Great Learner", "Solve 20 tsumego problems.", RewardPoints.HardReward, 20)
        {
        }
        public override Type GetViewModelToTry()
        {
            return typeof(TsumegoMenuViewModel);
        }
        public override bool NewTsumegoSolved() => true;
    }
}
