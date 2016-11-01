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

        public string Settings => LocalizeCaller();

        public string Library => LocalizeCaller();

        public string Load => LocalizeCaller();

        public string Loading => LocalizeCaller();

        public string Back => LocalizeCaller();

        public string Open => LocalizeCaller();

        public string Delete => LocalizeCaller();

        public string Select => LocalizeCaller();

        public string Rename => LocalizeCaller();

        public string UserInterface => LocalizeCaller();

        public string Sounds => LocalizeCaller();

        public string Language => LocalizeCaller();

        public string Gameplay => LocalizeCaller();

        public string Assistant => LocalizeCaller();

        public string FilterBySource => LocalizeCaller();

        public string LoadFolder => LocalizeCaller();

        public string DeleteSelection => LocalizeCaller();
    }
}
