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
        private const string AutoLanguageKey = "auto";

        /// <summary>
        /// Returns supported game languages in a dictionary. 
        /// Key is the culture tag
        /// </summary>
        public static readonly ReadOnlyDictionary<string, GameLanguage> SupportedLanguages =
            new ReadOnlyDictionary<string, GameLanguage>(new List<GameLanguage>()
            {
                new GameLanguage("AutoLanguage", AutoLanguageKey),
                new GameLanguage("English", "en"),
                new GameLanguage("Česky", "cs"),
                new GameLanguage("日本語", "jp")
            }.ToDictionary(
                language => language.CultureTag, //use culture tag as the dictionary key
                language => language
             ));

        /// <summary>
        /// Default language (auto)
        /// </summary>
        public static GameLanguage DefaultLanguage => SupportedLanguages[AutoLanguageKey];
    }
}
