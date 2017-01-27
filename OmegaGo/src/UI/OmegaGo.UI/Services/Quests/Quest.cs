using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Extensions;
using OmegaGo.UI.Services.Quests.IndividualQuests;

namespace OmegaGo.UI.Services.Quests
{
    public class Quest
    {
        public string Name { get; }
        public string Description { get; }
        public int PointReward { get; }

        protected Quest(string name, string description, int pointReward)
        {
            Name = name;
            Description = description;
            PointReward = pointReward;
        }

        public static ActiveQuest SpawnRandomQuest()
        {
            Type questType = Quest.allQuests.GetRandom();
            Quest quest = (Quest)Activator.CreateInstance(questType);
            return ActiveQuest.Create(quest);
        }

        private static List<Type> allQuests = new List<Type>
        {
            typeof(LearnerQuest)
        };
    }
}
