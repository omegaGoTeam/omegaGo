using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class HiddenQuest : Quest
    {
        public HiddenQuest() : base("Hidden Quest!", "Your quest transformed! Solve any one tsumego problem.", Points.EXTREME_REWARD, 1)
        {
        }

        public override bool NewTsumegoSolved() => true;
    }
}
