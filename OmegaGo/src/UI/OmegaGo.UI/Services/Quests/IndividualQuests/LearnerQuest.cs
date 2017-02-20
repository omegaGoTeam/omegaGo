using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class LearnerQuest : Quest
    {
        public LearnerQuest() : base("Learner", "Solve 5 tsumego problems.", Points.EASY_REWARD, 5)
        {

        }
        public override Type GetViewModelToTry()
        {
            return typeof(TsumegoMenuViewModel);
        }
        public override bool TryThisNowButtonVisible => true;

        public override bool NewTsumegoSolved() => true;
    }
}
