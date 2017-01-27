using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OmegaGo.UI.Services.Quests
{
    public class ActiveQuest
    {
        public string QuestID;

        [JsonIgnore]
        public Quest Quest => Quest.AllQuests[QuestID];
        public int Progress { get; set; }

        protected ActiveQuest() { }
        private ActiveQuest(string id, int progress)
        {
            QuestID = id;
            Progress = progress;
        }
        public static ActiveQuest Create(string id)
        {
            return new Quests.ActiveQuest(id, 0);
        }
    }
}
