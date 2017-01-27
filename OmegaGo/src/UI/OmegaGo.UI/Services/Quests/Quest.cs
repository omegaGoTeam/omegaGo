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
        public int MaximumProgress { get; }

        protected Quest(string name, string description, int pointReward, int maximumProgress)
        {
            Name = name;
            Description = description;
            PointReward = pointReward;
            MaximumProgress = maximumProgress;
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
        /*
    Challenges: Each challenge has the same chance to appear.

    • IGS Challenge: Play 3 games on the Internet Go Server.
    • OGS Challenge: Play 3 games on online-go.com.
    • Traditional: Play an online 19x19 game.
    • Human > Computer: Win 3 games against the artificial intelligence.
    • Epic: Play a game on the 25x25 board.
    • Learner: Try 5 tsumego problems.
    • Great Learner: Try 20 tsumego problems.
    • Master Learner: Solve, on the first try, 5 tsumego problems.
    • Blitz Rocket: Win a game, taking 5 minutes or less to do so.
    • Wisdom: Win a 19x19 online game that takes 1 hour or more.
    • Uneven Strength: Play 2 handicap games, online or against AI.
    • Getting stronger: Win against the hardest AI.
    • Pure Skill: Win against the hardest AI, without handicap.
    • Total Mastery: Win against the hardest AI, giving the AI a handicap of 3 stones.

*/
    }
}
