using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Quests.IndividualQuests;

namespace OmegaGo.UI.Services.Quests
{
    /// <summary>
    /// Quest used in XAML to have something displayed in XAML Designer.
    /// </summary>
    public class ExampleActiveQuest : ActiveQuest
    {
        public ExampleActiveQuest()
        {
            this.Progress = 1;
            this.QuestID = "Learner";
        }
    }
}
