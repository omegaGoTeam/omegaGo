using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OmegaGo.UI.Services.Settings
{
    /// <summary>
    /// Base class for JSON-serialized setting services
    /// </summary>
    public abstract class SettingsServiceBase : ISettingsService
    {
        public T GetSetting<T>( string key, Func<T> defaultValueBuilder, SettingLocality locality = SettingLocality.Local )
        {
            var retrievedSetting = RetrieveSetting( key, locality );
            if ( retrievedSetting != null )
            {               
                try
                {
                    return JsonConvert.DeserializeObject<T>( retrievedSetting );
                }
                catch
                {
                    //ignore, return default value instead
                }
            }
            return defaultValueBuilder();
        }

        /// <summary>
        /// Retrieves the setting using platform-specific settings service
        /// </summary>
        /// <param name="key">Key of the setting</param>
        /// <param name="localizty">Value of the setting</param>
        /// <returns></returns>
        protected abstract string RetrieveSetting( string key, SettingLocality localizty = SettingLocality.Local );
        
        public void SetSetting<T>( string key, T value, SettingLocality locality = SettingLocality.Local )
        {
            //serialize the setting
            var serializedValue = JsonConvert.SerializeObject( value );
            StoreSetting( key, serializedValue, locality );
        }

        /// <summary>
        /// Stores the setting using platform-specific settings service
        /// </summary>
        /// <param name="key">Key of the setting</param>
        /// <param name="value">Value of the setting</param>
        /// <param name="locality">Locality of the setting</param>
        protected abstract void StoreSetting( string key, string value, SettingLocality locality = SettingLocality.Local );
    }
}
