using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.Quests.IndividualQuests
{
    class HiddenQuest : Quest
    {
        public HiddenQuest() : base("Hidden Quest!", "Your quest transformed! Solve any one tsumego problem.", Points.EXTREME_REWARD, 1)
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
