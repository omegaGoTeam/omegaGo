using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Settings
{
    /// <summary>
    /// Base class for a group of related settings
    /// </summary>
    public abstract class SettingsGroup
    {
        /// <summary>
        /// Settings service
        /// </summary>
        private ISettingsService _service = null;

        /// <summary>
        /// Key of the group
        /// </summary>
        private readonly string _groupKey = null;

        /// <summary>
        /// Creates a settings group
        /// </summary>
        /// <param name="groupKey">Key of the group</param>
        /// <param name="service">Settings service used for storage</param>
        protected SettingsGroup(string groupKey, ISettingsService service)
        {
            if (groupKey == null) throw new ArgumentNullException(nameof(groupKey));
            if (service == null) throw new ArgumentNullException(nameof(service));
            _service = service;
            _groupKey = groupKey;
        }

        /// <summary>
        /// Creates a grouped setting key
        /// </summary>
        /// <param name="settingKey">Key of the setting</param>
        /// <returns>Grouped setting key</returns>
        internal string CreateGroupedSettingKey(string settingKey) => $"{_groupKey}_{settingKey}";

        /// <summary>
        /// Retrieves a stored setting by key
        /// </summary>
        /// <typeparam name="T">Type of the stored value</typeparam>
        /// <param name="key">Key of the setting</param>
        /// <param name="defaultValueBuilder">Default value provider</param>
        /// <param name="locality">Locality of the setting</param>
        /// <returns>Stored value</returns>
        protected T GetSetting<T>(string key, Func<T> defaultValueBuilder, SettingLocality locality = SettingLocality.Local) =>
            _service.GetSetting(CreateGroupedSettingKey(key), defaultValueBuilder, locality);

        /// <summary>
        /// Stores a setting by key
        /// </summary>
        /// <typeparam name="T">Type of the stored value</typeparam>
        /// <param name="key">Key of the setting</param>
        /// <param name="value">Value to store</param>
        /// <param name="locality">Locality of the setting</param>
        protected void SetSetting<T>(string key, T value, SettingLocality locality = SettingLocality.Local) =>
            _service.SetSetting(CreateGroupedSettingKey(key), value, locality);

        /// <summary>
        /// After setting retrieval, the setting is deserialized from JSON
        /// </summary>
        /// <typeparam name="T">Type of the setting</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValueBuilder">Value</param>
        /// <param name="locality">Locality</param>
        /// <returns></returns>
        protected T GetComplexSetting<T>(string key, Func<T> defaultValueBuilder,
            SettingLocality locality = SettingLocality.Local) where T : new()
            => _service.GetComplexSetting(CreateGroupedSettingKey(key), defaultValueBuilder, locality);

        /// <summary>
        /// Before storing the setting, it is first serialized to JSON
        /// </summary>
        /// <typeparam name="T">Type of the setting</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="locality">Locality</param>
        protected void SetComplexSetting<T>(string key, T value, SettingLocality locality = SettingLocality.Local)
            where T : new()
            => _service.SetComplexSetting(CreateGroupedSettingKey(key), value, locality);


        /// <summary>
        /// After setting retrieval from a file, the setting is deserialized from JSON
        /// </summary>
        /// <typeparam name="T">Type of the setting</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValueBuilder">Value</param>
        /// <param name="locality">Locality</param>
        /// <returns></returns>
        protected T GetLargeSetting<T>(string key, Func<T> defaultValueBuilder,
            SettingLocality locality = SettingLocality.Local) where T : new()
            => _service.GetLargeSetting(CreateGroupedSettingKey(key), defaultValueBuilder, locality);

        /// <summary>
        /// Before storing the setting into a file, it is first serialized to JSON
        /// </summary>
        /// <typeparam name="T">Type of the setting</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="locality">Locality</param>
        protected void SetLargeSetting<T>(string key, T value, SettingLocality locality = SettingLocality.Local)
            where T : new()
            => _service.SetLargeSetting(CreateGroupedSettingKey(key), value, locality);
    }
}
