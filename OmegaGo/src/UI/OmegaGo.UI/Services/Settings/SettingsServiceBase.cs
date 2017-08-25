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

        protected abstract string GetFileSettings(string key, SettingLocality locality = SettingLocality.Local);

        protected abstract void SetFileSettings(string key, string value, SettingLocality locality = SettingLocality.Local);

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

        /// <summary>
        /// After setting retrieval, the setting is deserialized from JSON
        /// </summary>
        /// <typeparam name="T">Type of the setting</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValueBuilder">Value</param>
        /// <param name="locality">Locality</param>
        /// <returns></returns>
        public T GetLargeSetting<T>(string key, Func<T> defaultValueBuilder, SettingLocality locality = SettingLocality.Local) where T : new()
        {
            var retrievedSetting = GetFileSettings(key, locality);
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
        public void SetLargeSetting<T>(string key, T value, SettingLocality locality = SettingLocality.Local) where T : new()
        {
            //serialize the setting
            var serializedValue = JsonConvert.SerializeObject(value);
            SetFileSettings(key, serializedValue, locality);
        }
    }
}
