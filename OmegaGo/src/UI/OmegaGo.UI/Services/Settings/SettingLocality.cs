using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Settings
{
    /// <summary>
    /// Indicates where the settings are stored
    /// </summary>
    public enum SettingLocality
    {
        /// <summary>
        /// Local setting
        /// </summary>
        Local,
        /// <summary>
        /// Setting roamed across devices
        /// </summary>
        Roamed
    }
}
