namespace OmegaGo.UI.Services.Settings
{
    public class AssistantSettings : SettingsGroup
    {
        public AssistantSettings(ISettingsService service) : base("Assistant", service)
        {
        }

        public string ProgramName {
            get { return GetSetting(nameof(ProgramName), () => "Random", SettingLocality.Roamed); }
            set { SetSetting(nameof(ProgramName), value, SettingLocality.Roamed); }
        }

        public bool EnableHints
        {
            get { return GetSetting(nameof(EnableHints), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(EnableHints), value, SettingLocality.Roamed); }
        }
        public bool EnableInOnlineGames
        {
            get { return GetSetting(nameof(EnableInOnlineGames), () => false, SettingLocality.Roamed); }
            set { SetSetting(nameof(EnableInOnlineGames), value, SettingLocality.Roamed); }
        }
    }
}