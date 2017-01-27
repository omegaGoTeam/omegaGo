using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.Services.Quests
{
    public class QuestEvents
    {
        private QuestsGroup questsGroup;

        public QuestEvents(QuestsGroup questsGroup)
        {
            this.questsGroup = questsGroup;
        }

        public void NewTsumegoSolved()
        {
            foreach(var quest in questsGroup.ActiveQuests.ToList())
            {
                if (quest.Quest.NewTsumegoSolved())
                {
                    questsGroup.ProgressQuest(quest, 1);
                }
            }
        }
    }
}
