using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OmegaGo.UI.Services.Settings
{
    /// <summary>
    /// Base class adding JSON-serialized settings
    /// </summary>
    public abstract class SettingsServiceBase : ISettingsService
    {
        public abstract T GetSetting<T>(string key, Func<T> defaultValueBuilder, SettingLocality locality = SettingLocality.Local);

        public abstract void SetSetting<T>(string key, T value, SettingLocality locality = SettingLocality.Local);

        /// <summary>
        /// After setting retrieval, the setting is deserialized from JSON
        /// </summary>
        /// <typeparam name="T">Type of the setting</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValueBuilder">Value</param>
        /// <param name="locality">Locality</param>
        /// <returns></returns>
        public T GetComplexSetting<T>(string key, Func<T> defaultValueBuilder, SettingLocality locality = SettingLocality.Local) where T : new()
        {
            var retrievedSetting = GetSetting<string>(key, () => null, locality);
            if (retrievedSetting != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(retrievedSetting);
                }
                catch
                {
                    //ignore, return default value instead
                }
            }
            return defaultValueBuilder();
        }

        /// <summary>
        /// Before storing the setting, it is first serialized to JSON
        /// </summary>
        /// <typeparam name="T">Type of the setting</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="locality">Locality</param>
        public void SetComplexSetting<T>(string key, T value, SettingLocality locality = SettingLocality.Local) where T : new()
        {
            //serialize the setting
            var serializedValue = JsonConvert.SerializeObject(value);
            SetSetting(key, serializedValue, locality);
        }
    }
}
