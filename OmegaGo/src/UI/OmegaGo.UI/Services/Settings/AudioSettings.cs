using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Settings
{
    public class AudioSettings : SettingsGroup
    {
        public AudioSettings(ISettingsService service) : base("Audio", service)
        {
        }

        public int MasterVolume
        {
            get { return GetSetting(nameof(MasterVolume), () => 100); }
            set { SetSetting(nameof(MasterVolume), value); }
        }
        public bool Mute
        {
            get { return GetSetting(nameof(Mute), () => false); }
            set { SetSetting(nameof(Mute), value); }
        }
        public int MusicVolume
        {
            get { return GetSetting(nameof(MusicVolume), () => 100); }
            set { SetSetting(nameof(MusicVolume), value); }
        }
        public int SfxVolume
        {
            get { return GetSetting(nameof(SfxVolume), () => 100); }
            set { SetSetting(nameof(SfxVolume), value); }
        }
        public bool PlayWhenYouPlaceStone
        {
            get { return GetSetting(nameof(PlayWhenYouPlaceStone), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(PlayWhenYouPlaceStone), value, SettingLocality.Roamed); }
        }
        public bool PlayWhenOthersPlaceStone
        {
            get { return GetSetting(nameof(PlayWhenOthersPlaceStone), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(PlayWhenOthersPlaceStone), value, SettingLocality.Roamed); }
        }
        public bool PlayWhenNotificationReceived
        {
            get { return GetSetting(nameof(PlayWhenNotificationReceived), () => true, SettingLocality.Roamed); }
            set { SetSetting(nameof(PlayWhenNotificationReceived), value, SettingLocality.Roamed); }
        }
    }
}
