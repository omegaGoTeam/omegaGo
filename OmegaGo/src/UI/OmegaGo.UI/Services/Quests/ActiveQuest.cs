using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Quests
{
    public class ActiveQuest
    {
        public Quest Quest;
        public int Progress;

        public static ActiveQuest Create(Quest quest)
        {
            return new Quests.ActiveQuest()
            {
                Quest = quest,
                Progress = 0
            };
        }
    }
}
