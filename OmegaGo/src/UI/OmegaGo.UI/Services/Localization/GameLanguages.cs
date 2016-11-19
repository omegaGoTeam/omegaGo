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
        public static readonly ReadOnlyDictionary<string, GameLanguage> SupportedLanguages =
            new ReadOnlyDictionary<string, GameLanguage>( new Dictionary<string, GameLanguage>()
            {
                { "auto", new GameLanguage( "AutoLanguage", "auto" ) },
                { "en", new GameLanguage( "English", "en" ) },
                { "cs", new GameLanguage( "Česky", "cs" ) }
            } );

        /// <summary>
        /// Default language (auto)
        /// </summary>
        public static GameLanguage DefaultLanguage => SupportedLanguages[ "auto" ];
    }
}
