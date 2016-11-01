using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Localization
{
    /// <summary>
    /// Simple to use localization service for a given resource manager
    /// Can be derived from to create an app specific strongly typed localization service
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        /// <summary>
        /// Underlying resource manager
        /// </summary>
        private readonly ResourceManager _resourceManager;

        /// <summary>
        /// Localization service constructor
        /// </summary>
        /// <param name="resourceManager">Resource manager for the required resource file</param>
        public LocalizationService(ResourceManager resourceManager)
        {
            if (resourceManager == null) throw new ArgumentNullException(nameof(resourceManager));

            _resourceManager = resourceManager;
        }

        /// <summary>
        /// Provides easy access to translation of a given key
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <returns>Localization or key if not found</returns>
        public string this[string key] => GetString(key);

        /// <summary>
        /// Returns a translation for a given resource key under default thread culture
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Localization or key if not found</returns>
        public string GetString(string key = "") => GetString(key, CultureInfo.CurrentUICulture );


        /// <summary>
        /// Returns a translation for a given resource key in given culture
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <param name="culture">Culture</param>
        /// <returns>Localization or key if not found</returns>
        public string GetString( string key, CultureInfo culture )
        {
            if ( key == null ) throw new ArgumentNullException( nameof( key ) );
            if ( culture == null ) throw new ArgumentNullException( nameof( culture ) );

            string result = _resourceManager.GetString( key, culture );
            return result ?? key;
        }

        /// <summary>
        /// Automatically localizes based on the caller's name
        /// Intended for use in derived classes for strongly typed property localization
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <returns>Localization of key if not found</returns>
        protected string LocalizeCaller( [CallerMemberName] string key = null ) => GetString( key );
    }
}
