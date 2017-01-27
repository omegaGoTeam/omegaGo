using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Quests;

namespace OmegaGo.UI.Services.Settings
{
    /// <summary>
    /// Manages Tsumego related settings
    /// </summary>
    public class QuestsGroup : SettingsGroup
    {
        public QuestsGroup(ISettingsService service) : base("Quests", service)
        {
        }
        
        public DateTime LastQuestReceivedWhen
        {
            get { return GetSetting(nameof(LastQuestReceivedWhen), ()=>DateTime.Now.AddDays(-1)); }
            set { SetSetting(nameof(LastQuestReceivedWhen), value); }
        }
        public DateTime LastQuestExchangedWhen
        {
            get { return GetSetting(nameof(LastQuestExchangedWhen), () => DateTime.Now.AddDays(-1)); }
            set { SetSetting(nameof(LastQuestExchangedWhen), value); }
        }

        public int Points
        {
            get { return GetSetting(nameof(Points), () => 0); }
            set { SetSetting(nameof(Points), value); }
        }

        private List<ActiveQuest> _activeQuests;

        private List<ActiveQuest> ActiveQuests
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
        public void SaveChanges()
        {
            if (_activeQuests == null)
            {
                _activeQuests = GetComplexSetting(nameof(this.ActiveQuests),
                      () => new List<ActiveQuest>());
            }
            SetComplexSetting(nameof(this.ActiveQuests), _activeQuests);
        }

    }
}
