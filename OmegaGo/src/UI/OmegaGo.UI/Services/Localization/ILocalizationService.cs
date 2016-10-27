using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Localization
{
    /// <summary>
    /// Provides localization services
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Provides localization of a given key
        /// </summary>
        /// <param name="key">Key to localize</param>
        /// <returns>Localization of the key</returns>
        string this[ string key ] { get; }
    }
}
