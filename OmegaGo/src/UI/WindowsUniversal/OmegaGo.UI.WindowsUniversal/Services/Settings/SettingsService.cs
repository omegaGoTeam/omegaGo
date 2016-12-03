using OmegaGo.UI.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace OmegaGo.UI.WindowsUniversal.Services.Settings
{
    /// <summary>
    /// UWP implementation of settings service using <see cref="ApplicationData">ApplicationData</see>
    /// </summary>
    internal class SettingsService : SettingsServiceBase
    {
        public override T GetSetting<T>(string key, Func<T> defaultValueBuilder, SettingLocality locality = SettingLocality.Local)
        {
            var container = locality == SettingLocality.Roamed ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            if (container.Values.ContainsKey(key))
            {
                //get existing
                try
                {
                    return (T)container.Values[key];
                }
                catch
                {
                    //invalid value for the given typ, remove
                    container.Values.Remove(key);
                }
            }
            return defaultValueBuilder();
        }

        public override void SetSetting<T>(string key, T value, SettingLocality locality = SettingLocality.Local)
        {            
            var container = locality == SettingLocality.Roamed ? ApplicationData.Current.RoamingSettings : ApplicationData.Current.LocalSettings;
            if (container.Values.ContainsKey(key))
            {
                container.Values[key] = value;
            }
            else
            {               
                container.Values.Add(key, value);
            }
        }
    }
}
