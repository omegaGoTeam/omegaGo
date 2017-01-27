namespace OmegaGo.UI.Services.Settings
{
    public class StatisticsRecords : SettingsGroup
    {
        public StatisticsRecords(ISettingsService service) : base("Statistics", service)
        {
        }
        public int QuestsCompleted
        {
            get { return GetSetting(nameof(QuestsCompleted), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(QuestsCompleted), value, SettingLocality.Roamed); }
        }
        public int HotseatGamesPlayed {
            get { return GetSetting(nameof(HotseatGamesPlayed), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(HotseatGamesPlayed), value, SettingLocality.Roamed); }
        }
        public int OnlineGamesPlayed
        {
            get { return GetSetting(nameof(OnlineGamesPlayed), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(OnlineGamesPlayed), value, SettingLocality.Roamed); }
        }
        public int LocalGamesPlayed
        {
            get { return GetSetting(nameof(LocalGamesPlayed), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(LocalGamesPlayed), value, SettingLocality.Roamed); }
        }
        public int OnlineGamesWon
        {
            get { return GetSetting(nameof(OnlineGamesWon), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(OnlineGamesWon), value, SettingLocality.Roamed); }
        }
        public int LocalGamesWon
        {
            get { return GetSetting(nameof(LocalGamesWon), () => 0, SettingLocality.Roamed); }
            set { SetSetting(nameof(LocalGamesWon), value, SettingLocality.Roamed); }
        }

    }
}