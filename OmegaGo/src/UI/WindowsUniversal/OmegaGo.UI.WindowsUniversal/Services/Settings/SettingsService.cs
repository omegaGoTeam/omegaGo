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
        private readonly Dictionary<string, object> _settingCache = new Dictionary<string, object>();

        /// <summary>
        /// Gets a application setting
        /// </summary>
        /// <typeparam name="T">Type of setting value</typeparam>
        /// <param name="key">Setting key</param>
        /// <param name="defaultValueBuilder">Default value builder</param>
        /// <param name="locality">Locality of the setting</param>
        /// <returns></returns>
        public override T GetSetting<T>(
            string key,
            Func<T> defaultValueBuilder,
            SettingLocality locality = SettingLocality.Local)
        {
            object result = null;
            if ( !_settingCache.TryGetValue(key, out result))
            {
                var container = locality == SettingLocality.Roamed ?
                ApplicationData.Current.RoamingSettings :
                ApplicationData.Current.LocalSettings;
                _settingCache[key] = RetrieveSettingFromApplicationData(key, defaultValueBuilder, container);
            }
            return (T)_settingCache[key];
        }

        /// <summary>
        /// Retrieves a setting from Application Data
        /// </summary>
        /// <typeparam name="T">Type of the setting to retrieve</typeparam>
        /// <param name="key">Setting key</param>
        /// <param name="defaultValueBuilder">Default value builder</param>
        /// <param name="container">Container to access</param>
        /// <returns></returns>
        private T RetrieveSettingFromApplicationData<T>(
           string key,
           Func<T> defaultValueBuilder,
           ApplicationDataContainer container)
        {
            object result = null;
            if (container.Values.TryGetValue(key, out result))
            {
                //get existing
                try
                {
                    return (T)result;
                }
                catch
                {
                    //invalid value for the given type, remove
                    container.Values.Remove(key);
                }
            }
            return defaultValueBuilder();
        }

        /// <summary>
        /// Stores a setting
        /// </summary>
        /// <typeparam name="T">Type of the setting value</typeparam>
        /// <param name="key">Setting key</param>
        /// <param name="value">Value to store</param>
        /// <param name="locality">Setting locality</param>
        public override void SetSetting<T>(
           string key,
           T value,
           SettingLocality locality = SettingLocality.Local)
        {
            var container = locality == SettingLocality.Roamed ?
               ApplicationData.Current.RoamingSettings :
               ApplicationData.Current.LocalSettings;
            container.Values[key] = value;
            //ensure cache is invalidated
            _settingCache.Remove(key);
        }
    }
}
