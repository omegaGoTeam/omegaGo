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

        public int FluffyDepth
        {
            get { return GetSetting(nameof(FluffyDepth), () => 3, SettingLocality.Roamed); }
            set { SetSetting(nameof(FluffyDepth), value, SettingLocality.Roamed); }
        }
        public int FuegoMaxGames
        {
            get { return GetSetting(nameof(FuegoMaxGames), () => 100000, SettingLocality.Roamed); }
            set { SetSetting(nameof(FuegoMaxGames), value, SettingLocality.Roamed); }
        }
        public bool FuegoPonder
        {
            get { return GetSetting(nameof(FuegoPonder), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(FuegoPonder), value, SettingLocality.Roamed); }
        }
        public bool FuegoAllowResign
        {
            get { return GetSetting(nameof(FuegoAllowResign), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(FuegoAllowResign), value, SettingLocality.Roamed); }
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