using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Localization
{
    /// <summary>
    /// Game languages
    /// </summary>
    public static class GameLanguages
    {
        /// <summary>
        /// Returns supported game languages
        /// </summary>
        public static readonly ReadOnlyCollection<string> SupportedLanguages =
            new ReadOnlyCollection<string>( new List<string>()
            {
                "AutoLanguage",
                "English",
                "Česky"
            } );

        /// <summary>
        /// Default language (auto)
        /// </summary>
        public static string DefaultLanguage => SupportedLanguages.First();
    }
}
