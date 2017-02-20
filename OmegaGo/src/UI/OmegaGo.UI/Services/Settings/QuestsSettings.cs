using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Quests;

namespace OmegaGo.UI.Services.Settings
{
    /// <summary>
    /// Manages Tsumego related settings
    /// </summary>
    public class QuestsSettings : SettingsGroup
    {
        public QuestsSettings(ISettingsService service) : base("Quests", service)
        {
            Events = new Quests.QuestEvents(this);
        }
        
        public DateTime LastQuestReceivedWhen
        {
            get
            {
                return GetComplexSetting(nameof(LastQuestReceivedWhen), () => DateTime.Today.GetNoon().GetPreviousDay());
            }
            set { SetComplexSetting(nameof(LastQuestReceivedWhen), value); }
        }
        public DateTime LastQuestExchangedWhen
        {
            get
            {
                return GetComplexSetting(nameof(LastQuestExchangedWhen), () => DateTime.Today.GetNoon().GetPreviousDay());
            }
            set { SetComplexSetting(nameof(LastQuestExchangedWhen), value); }
        }

        public int Points
        {
            get { return GetSetting(nameof(Points), () => 0); }
            set { SetSetting(nameof(Points), value); }
        }

        private List<ActiveQuest> _activeQuests;

        public IEnumerable<ActiveQuest> ActiveQuests
        {
            get
            {
                if (_activeQuests == null)
                {
                    _activeQuests = GetComplexSetting(nameof(this.ActiveQuests),
                        () => new List<ActiveQuest>());
                }
                return _activeQuests;
            }
        }
        private void SaveChanges()
        {
            if (_activeQuests == null)
            {
                _activeQuests = GetComplexSetting(nameof(this.ActiveQuests),
                      () => new List<ActiveQuest>());
            }
            SetComplexSetting(nameof(this.ActiveQuests), _activeQuests);
        }

        public void AddQuest(ActiveQuest quest)
        {
            _activeQuests.Add(quest);
            SaveChanges();
        }
        public void ProgressQuest(ActiveQuest quest, int additionalProgress)
        {
            quest.Progress += additionalProgress;
            if (quest.Progress >= quest.Quest.MaximumProgress)
            {
                LoseQuest(quest);
                Points += quest.Quest.PointReward;
                Mvx.Resolve<IGameSettings>().Statistics.QuestsCompleted++;
            }
            SaveChanges();
        }
        public void LoseQuest(ActiveQuest quest)
        {
            _activeQuests.Remove(quest);
            SaveChanges();
        }

        public void ClearAllQuests()
        {
            _activeQuests = new List<Quests.ActiveQuest>();
            SaveChanges();
        }

        public QuestEvents Events { get; }
    
    }
}
