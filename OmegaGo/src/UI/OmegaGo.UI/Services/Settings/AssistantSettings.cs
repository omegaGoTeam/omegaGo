namespace OmegaGo.UI.Services.Settings
{
    public class AssistantSettings : SettingsGroup
    {
        public AssistantSettings(ISettingsService service) : base("Assistant", service)
        {
        }

        public string ProgramName {
            get { return GetSetting(nameof(ProgramName), () => "Random"); }
            set { SetSetting(nameof(ProgramName), value); }
        }

        public bool EnableHints
        {
            get { return GetSetting(nameof(EnableHints), () => true); }
            set { SetSetting(nameof(EnableHints), value); }
        }
        public bool EnableInOnlineGames
        {
            get { return GetSetting(nameof(EnableInOnlineGames), () => false); }
            set { SetSetting(nameof(EnableInOnlineGames), value); }
        }
    }
}