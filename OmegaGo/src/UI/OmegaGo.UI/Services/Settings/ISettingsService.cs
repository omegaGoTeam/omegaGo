using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Settings
{
    /// <summary>
    /// Facilitates application settings management
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Retrieves a stored setting by key
        /// </summary>
        /// <typeparam name="T">Type of the stored value</typeparam>
        /// <param name="key">Key of the setting</param>
        /// <param name="defaultValueBuilder">Default value provider</param>
        /// <param name="locality">Locality of the setting</param>
        /// <returns></returns>
        T GetSetting<T>(string key, Func<T> defaultValueBuilder, SettingLocality locality = SettingLocality.Local);

        /// <summary>
        /// Stores a setting by key
        /// </summary>
        /// <typeparam name="T">Type of the stored value</typeparam>
        /// <param name="key">Key of the setting</param>
        /// <param name="value">Value to store</param>
        /// <param name="locality">Locality of the setting</param>
        void SetSetting<T>(string key, T value, SettingLocality locality = SettingLocality.Local);

        /// <summary>
        /// After setting retrieval, the setting is deserialized from JSON
        /// </summary>
        /// <typeparam name="T">Type of the setting</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValueBuilder">Value</param>
        /// <param name="locality">Locality</param>
        /// <returns></returns>
        T GetComplexSetting<T>(string key, Func<T> defaultValueBuilder,
            SettingLocality locality = SettingLocality.Local) where T : new();

        /// <summary>
        /// Before storing the setting, it is first serialized to JSON
        /// </summary>
        /// <typeparam name="T">Type of the setting</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="locality">Locality</param>
        void SetComplexSetting<T>(string key, T value, SettingLocality locality = SettingLocality.Local)
            where T : new();
    }
}
