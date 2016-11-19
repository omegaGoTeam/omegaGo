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
        public static readonly ReadOnlyCollection<GameLanguage> SupportedLanguages =
            new ReadOnlyCollection<GameLanguage>( new List<GameLanguage>()
            {
                new GameLanguage() { Name = "AutoLanguage" },
                new GameLanguage() { CultureTag = "en", Name=  "English" },
                new GameLanguage() {CultureTag= "cs", Name= "Česky" }
            } );

        /// <summary>
        /// Default language (auto)
        /// </summary>
        public static GameLanguage DefaultLanguage => SupportedLanguages.First();
    }
}
