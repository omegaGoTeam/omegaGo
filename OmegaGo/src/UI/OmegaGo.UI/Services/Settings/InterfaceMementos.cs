namespace OmegaGo.UI.Services.Settings
{
    public class InterfaceMementos : SettingsGroup
    {
        public InterfaceMementos(ISettingsService service) : base("InterfaceMementos", service)
        {
        }
        public int BoardWidth
        {
            get { return GetSetting(nameof(BoardWidth), () => 19); }
            set { SetSetting(nameof(BoardWidth), value); }
        }
        public int BoardHeight
        {
            get { return GetSetting(nameof(BoardHeight), () => 19); }
            set { SetSetting(nameof(BoardHeight), value); }
        }
        public string IgsName
        {
            get { return GetSetting(nameof(IgsName), () => "OmegaGo1"); }
            set { SetSetting(nameof(IgsName), value); }
        }
        public string IgsPassword
        {
            get
            {
                if (IgsRememberPassword)
                {
                    return GetSetting(nameof(IgsPassword), () => "123456789");
                }
                else
                {
                    return "";
                }
            }
            set { SetSetting(nameof(IgsPassword), value); }
        }
        public bool IgsRememberPassword
        {
            get { return GetSetting(nameof(IgsRememberPassword), () => true); }
            set { SetSetting(nameof(IgsRememberPassword), value); }
        }
        public bool IgsAutoLogin
        {
            get { return GetSetting(nameof(IgsAutoLogin), () => false); }
            set { SetSetting(nameof(IgsAutoLogin), value); }
        }
        public string KgsName
        {
            get { return GetSetting(nameof(KgsName), () => "OmegaGo1"); }
            set { SetSetting(nameof(KgsName), value); }
        }
        public string KgsPassword
        {
            get
            {
                if (KgsRememberPassword)
                {
                    return GetSetting(nameof(KgsPassword), () => "123456789");
                }
                else
                {
                    return "";
                }
            }
            set { SetSetting(nameof(KgsPassword), value); }
        }
        public bool KgsRememberPassword
        {
            get { return GetSetting(nameof(KgsRememberPassword), () => true); }
            set { SetSetting(nameof(KgsRememberPassword), value); }
        }
        public bool KgsAutoLogin
        {
            get { return GetSetting(nameof(KgsAutoLogin), () => false); }
            set { SetSetting(nameof(KgsAutoLogin), value); }
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


    }
}