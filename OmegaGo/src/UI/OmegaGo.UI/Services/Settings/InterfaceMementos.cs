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
    }
}