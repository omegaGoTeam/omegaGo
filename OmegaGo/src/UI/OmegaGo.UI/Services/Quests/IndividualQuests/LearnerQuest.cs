using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class LearnerQuest : Quest
    {
        public LearnerQuest() : base("Learner", "Try solving 5 tsumego problems.", Points.EASY_REWARD)
        {
        }
    }
}
