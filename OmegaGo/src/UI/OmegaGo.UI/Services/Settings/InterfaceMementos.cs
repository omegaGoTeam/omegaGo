using MvvmCross.Platform;
using OmegaGo.UI.Services.PasswordVault;

namespace OmegaGo.UI.Services.Settings
{
    public class InterfaceMementos : SettingsGroup
    {
        public InterfaceMementos(ISettingsService service) : base("InterfaceMementos", service)
        {
        }
        public int BoardWidth
        {
            get { return GetSetting(nameof(BoardWidth), () => 19, SettingLocality.Roamed); }
            set { SetSetting(nameof(BoardWidth), value, SettingLocality.Roamed); }
        }
        public int BoardHeight
        {
            get { return GetSetting(nameof(BoardHeight), () => 19, SettingLocality.Roamed); }
            set { SetSetting(nameof(BoardHeight), value, SettingLocality.Roamed); }
        }
        public string IgsName
        {
            get { return GetSetting(nameof(IgsName), () => "OmegaGo1", SettingLocality.Roamed); }
            set { SetSetting(nameof(IgsName), value, SettingLocality.Roamed); }
        }
        public string IgsPassword
        {
            get
            {
                if (IgsRememberPassword)
                {
                    return GetSetting(nameof(IgsPassword), () => "123456789", SettingLocality.Roamed);
                }
                else
                {
                    return "";
                }
            }
            set { SetSetting(nameof(IgsPassword), value, SettingLocality.Roamed); }
        }
        public bool IgsRememberPassword
        {
            get { return GetSetting(nameof(IgsRememberPassword), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(IgsRememberPassword), value, SettingLocality.Roamed); }
        }
        public bool IgsAutoLogin
        {
            get { return GetSetting(nameof(IgsAutoLogin), () => false, SettingLocality.Roamed); }
            set { SetSetting(nameof(IgsAutoLogin), value, SettingLocality.Roamed); }
        }
        public string KgsName
        {
            get { return GetSetting(nameof(KgsName), () => "OmegaGo1", SettingLocality.Roamed); }
            set { SetSetting(nameof(KgsName), value, SettingLocality.Roamed); }
        }
        public string KgsPassword
        {
            get
            {
                if (KgsRememberPassword)
                {
                    return GetSetting(nameof(KgsPassword), () => "123456789", SettingLocality.Roamed);
                }
                else
                {
                    return "";
                }
            }
            set { SetSetting(nameof(KgsPassword), value, SettingLocality.Roamed); }
        }
        public bool KgsRememberPassword
        {
            get { return GetSetting(nameof(KgsRememberPassword), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(KgsRememberPassword), value, SettingLocality.Roamed); }
        }
        public bool KgsAutoLogin
        {
            get { return GetSetting(nameof(KgsAutoLogin), () => false, SettingLocality.Roamed); }
            set { SetSetting(nameof(KgsAutoLogin), value, SettingLocality.Roamed); }
        }
    }
}