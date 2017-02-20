using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.UI.Services.Quests.IndividualQuests;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.Quests
{
    public abstract class Quest
    {
        public string Name { get; }
        public string Description { get; }
        public int PointReward { get; }
        public int MaximumProgress { get; }

        public virtual bool TryThisNowButtonVisible => false;

    /// <summary>
        /// Prepares us for jump to a viewmodel where we can make progress on this quest, then returns the type
        /// of the viewmodel to jump to. The preparation would usually consist of preparing the GameCreationBundle.
        /// </summary>
        /// <returns></returns>
        public virtual Type GetViewModelToTry()
        {
            return null;
        }

        protected Quest(string name, string description, int pointReward, int maximumProgress)
        {
            Name = name;
            Description = description;
            PointReward = pointReward;
            MaximumProgress = maximumProgress;
        }

        public static ActiveQuest SpawnRandomQuest(IEnumerable<string> avoidTheseQuests)
        {
            List<string> questsToAvoid = avoidTheseQuests.ToList();
            string randomQuestName = Quest.AllQuests.Keys.Where(key => !questsToAvoid.Contains(key)).ToList().GetRandom();
            return ActiveQuest.Create(randomQuestName);
        }

        public static Dictionary<string, Quest> AllQuests = new Dictionary<string, Quest>
        {
            ["IgsChallenge"] = new IgsChallengeQuest(),
            ["KgsChallenge"] = new KgsChallengeQuest(),
            ["TraditionalQuest"] = new TraditionalQuest(),
            ["OnlineTraditionalQuest"] = new OnlineTraditionalQuest(),
            ["SoloVictoriesQuest"] = new SoloVictoriesQuest(),
            ["LearnerQuest"] = new LearnerQuest(),
            ["GreatLearnerQuest"] = new GreatLearnerQuest(),
            ["MasterLearnerQuest"] = new MasterLearnerQuest(),
            ["BlitzRocketQuest"] = new BlitzRocketQuest(),
            ["WisdomQuest"] = new WisdomQuest(),
            ["UnevenStrengthQuest"] = new UnevenStrengthQuest(),
            ["GettingStrongerQuest"] = new GettingStrongerQuest(),
            ["PureSkillQuest"] = new PureSkillQuest(),
            ["TotalMasteryQuest"] = new TotalMasteryQuest(),
            [HiddenQuestKey] = new HiddenQuest() 
        };


        public const string HiddenQuestKey = "HIDDEN";
        

        public const int MAXIMUM_NUMBER_OF_QUESTS = 3;
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
        public virtual bool NewTsumegoSolved() => false;

        /// <summary>
        /// Returns a value indicating whether the fact that the user just finished a game they were actually playing in should 
        /// advance progress on this quest.
        /// </summary>
        public virtual bool GameCompleted(GameCompletedQuestInformation info) => false;
    }
}
