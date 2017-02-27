using System;
using System.Collections.Generic;
using MvvmCross.Platform;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Services.Quests;

namespace OmegaGo.UI.Services.Settings
{  
    /// <summary>
    /// Manages Tsumego related settings
    /// </summary>
    public class QuestsSettings : SettingsGroup
    {
        private List<ActiveQuest> _activeQuests;

        public QuestsSettings(ISettingsService service) : base("Quests", service)
        {
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
            set
            {
                SetSetting(nameof(Points), value);
            }
        }

        public IEnumerable<ActiveQuest> ActiveQuests
        {
            get
            {
                if (_activeQuests == null)
                {
                    _activeQuests = GetComplexSetting(nameof(ActiveQuests),
                        () => new List<ActiveQuest>());
                }
                return _activeQuests;
            }
        }        

        public void AddQuest(ActiveQuest quest)
        {
            _activeQuests.Add(quest);
            SaveChanges();
        }
        
        public void LoseQuest(ActiveQuest quest)
        {
            _activeQuests.Remove(quest);
            SaveChanges();
        }

        public void ClearAllQuests()
        {
            _activeQuests = new List<ActiveQuest>();
            SaveChanges();
        }

        private void SaveChanges()
        {
            if (_activeQuests == null)
            {
                _activeQuests = GetComplexSetting(nameof(ActiveQuests),
                      () => new List<ActiveQuest>());
            }
            SetComplexSetting(nameof(ActiveQuests), _activeQuests);
        }
    }
}
