using OmegaGo.UI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Localization
{
    /// <summary>
    /// Default localizer for the LocalizedStrings resources
    /// </summary>
    public class Localizer : LocalizationService
    {
        /// <summary>
        /// Initializes localizer
        /// </summary>
        public Localizer() : base(LocalizedStrings.ResourceManager)
        {
            
        }

        public string BoardSize => LocalizeCaller();
    }
}
