using System;
using System.Collections.Generic;
using MvvmCross.Platform;
using OmegaGo.UI.Extensions;
using OmegaGo.UI.Services.Localization;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Services.Quests;

namespace OmegaGo.UI.Services.Settings
{
    //TODO Martin : Separate Quest notification logic from this class - it should be just a setting container
    /// <summary>
    /// Manages Tsumego related settings
    /// </summary>
    public class QuestsSettings : SettingsGroup
    {
        private List<ActiveQuest> _activeQuests;

        public QuestsSettings(ISettingsService service) : base("Quests", service)
        {
            Events = new QuestEvents(this);

        }

        public QuestEvents Events { get; }

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
                int oldvalue = GetSetting(nameof(Points), () => 0);
                if (value > oldvalue)
                {
                    var notificationService = Mvx.Resolve<INotificationService>();
                    var localizationService = Mvx.Resolve<ILocalizationService>();
                    int gain = value - oldvalue;
                    notificationService.TriggerNotification(new BubbleNotification(String.Format(localizationService["YouHaveGainedXPointsNowYouHaveY"], gain, value)));
                    if (Ranks.AdvancedInRank(oldvalue, value))
                    {
                        notificationService.TriggerNotification(new BubbleNotification(String.Format(localizationService["YouHaveAdvancedToNewRankX"], Ranks.GetRankName(localizationService, value))));
                    }
                }
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
