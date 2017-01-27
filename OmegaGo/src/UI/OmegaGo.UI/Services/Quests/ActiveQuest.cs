using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Quests
{
    public class ActiveQuest
    {
        public Quest Quest { get; set; }
        public int Progress { get; set; }

        protected ActiveQuest() { }
        private ActiveQuest(Quest quest, int progress)
        {
            Quest = quest;
            Progress = progress;
        }
        public static ActiveQuest Create(Quest quest)
        {
            return new Quests.ActiveQuest(quest, 0);
        }
    }
}
