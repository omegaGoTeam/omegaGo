using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OmegaGo.UI.Services.Quests
{
    /// <summary>
    /// An <see cref="ActiveQuest"/> is information about a quest the user currently has and
    /// is progressing in. A list of these is serialized in settings. 
    /// </summary>
    public class ActiveQuest
    {
        /// <summary>
        /// ID of the quest description. This is serialized and this is how quests are found
        /// upon reload. It should be language-independent. (TODO make it so)
        /// 
        /// Must be public because of serialization.
        /// </summary>
        public string QuestID { get; set; }

        [JsonIgnore]
        public Quest Quest => Quest.AllQuests[QuestID];
        /// <summary>
        /// Gets or sets the player's progress on this quest. When the progress reaches the
        /// quest's maximum, the quest is deemed completed.
        /// </summary>
        public int Progress { get; set; }
        /// <summary>
        /// Used by <see cref="ExampleActiveQuest"/> only. 
        /// </summary>
        protected ActiveQuest() { }
        private ActiveQuest(string id, int progress)
        {
            QuestID = id;
            Progress = progress;
        }
        /// <summary>
        /// Creates a new <see cref="ActiveQuest"/> connected to a <see cref="Quest"/> with
        /// the specified language-independent ID.  
        /// </summary>
        /// <param name="id">The quest type's identifier.</param>
        public static ActiveQuest Create(string id)
        {
            return new Quests.ActiveQuest(id, 0);
        }
    }
}
