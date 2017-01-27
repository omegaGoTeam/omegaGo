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
        private QuestsSettings _questsSettings;

        public QuestEvents(QuestsSettings _questsSettings)
        {
            this._questsSettings = _questsSettings;
        }

        public void NewTsumegoSolved()
        {
            foreach(var quest in this._questsSettings.ActiveQuests.ToList())
            {
                if (quest.Quest.NewTsumegoSolved())
                {
                    this._questsSettings.ProgressQuest(quest, 1);
                }
            }
        }
    }
}
