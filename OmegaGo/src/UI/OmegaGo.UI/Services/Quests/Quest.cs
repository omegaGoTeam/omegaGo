using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using Newtonsoft.Json;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Quests.IndividualQuests;
using OmegaGo.UI.ViewModels;

namespace OmegaGo.UI.Services.Quests
{
    /// <summary>
    /// Represents a singleplayer quest
    /// </summary>
    public abstract class Quest
    {
        /// <summary>
        /// Key of the hidden quest
        /// </summary>
        public const string HiddenQuestKey = "HIDDEN";

        /// <summary>
        /// Maximum number of quests
        /// </summary>
        public const int MaximumNumberOfQuests = 3;

        /// <summary>
        /// Definitions of all quests
        /// </summary>
        private static readonly Dictionary<string, Quest> _allQuests = new Dictionary<string, Quest>
        {
            ["IgsChallenge"] = new IgsChallengeQuest(),
            ["KgsChallenge"] = new KgsChallengeQuest(),
            ["TraditionalQuest"] = new TraditionalQuest(),
            ["OnlineTraditionalQuest"] = new OnlineTraditionalQuest(),
            ["SoloVictoriesQuest"] = new SoloVictoriesQuest(),
            ["LearnerQuest"] = new LearnerQuest(),
            ["GreatLearnerQuest"] = new GreatLearnerQuest(),
            ["UnevenStrengthQuest"] = new UnevenStrengthQuest(),
            ["GettingStrongerQuest"] = new GettingStrongerQuest(),
            ["PureSkillQuest"] = new PureSkillQuest(),
            ["TotalMasteryQuest"] = new TotalMasteryQuest(),
            [HiddenQuestKey] = new HiddenQuest()
        };

        private string _fallbackName;
        private string _fallbackDescription;

        /// <summary>
        /// Creates a quest
        /// </summary>
        /// <param name="name">Name of the quest</param>
        /// <param name="description">Description of the quest</param>
        /// <param name="pointReward">Point reward</param>
        /// <param name="maximumProgress">Maximum reachable progress</param>
        protected Quest(string name, string description, int pointReward, int maximumProgress)
        {
            _fallbackName = name;
            _fallbackDescription = description;
            PointReward = pointReward;
            MaximumProgress = maximumProgress;
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                string id = this.GetType().Name + "_Name";
                string trans = ((Localizer) Mvx.Resolve<ILocalizationService>()).GetString(id);
                if (trans == id)
                {
                    return "[" + _fallbackName + "]";
                }
                return trans;
            }

        }

        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get
            {
                string id = this.GetType().Name + "_Description";
                string trans = ((Localizer)Mvx.Resolve<ILocalizationService>()).GetString(id);
                if (trans == id)
                {
                    return "[" + _fallbackDescription + "]";
                }
                return trans;
            }

        }
        /// <summary>
        /// Number of points rewarded for this quest
        /// </summary>
        public int PointReward { get; }

        /// <summary>
        /// Maximum progress reachable
        /// </summary>
        public int MaximumProgress { get; }

        /// <summary>
        /// Indicates whether the try this now button should be displayed
        /// </summary>
        public abstract bool TryThisNowButtonVisible { get; }

        /// <summary>
        /// Dictionary of all quests
        /// </summary>
        public static IReadOnlyDictionary<string, Quest> AllQuests => new ReadOnlyDictionary<string, Quest>(_allQuests);

        /// <summary>
        /// Spawns a random quests
        /// </summary>
        /// <param name="avoidTheseQuests">Quests to be avoided</param>
        /// <returns>Random quest</returns>
        public static ActiveQuest SpawnRandomQuest(IEnumerable<string> avoidTheseQuests)
        {
            List<string> questsToAvoid = avoidTheseQuests.ToList();
            string randomQuestName = Quest.AllQuests.Keys.Where(key => !questsToAvoid.Contains(key)).ToList().GetRandom();
            return ActiveQuest.Create(randomQuestName);
        }

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
        
        /// <summary>
        /// Prepares us for jump to a viewmodel where we can make progress on this quest, then returns the type
        /// of the viewmodel to jump to. The preparation would usually consist of preparing the GameCreationBundle.
        /// </summary>
        /// <returns></returns>
        public virtual Type GetViewModelToTry()
        {
            return null;
        }

        /// <summary>
        /// Returns the value indicating whether the fact that the user just solved a tsumego problem should advance progress on
        /// this quest
        /// </summary>
        /// <returns></returns>
        public virtual bool NewTsumegoSolved() => false;

        /// <summary>
        /// Returns a value indicating whether the fact that the user just finished a game they were actually playing in should 
        /// advance progress on this quest.
        /// </summary>
        public virtual bool GameCompleted(GameCompletedQuestInformation info) => false;
    }
}
