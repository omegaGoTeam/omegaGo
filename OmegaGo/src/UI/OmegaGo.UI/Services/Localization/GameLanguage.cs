using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Localization
{
    /// <summary>
    /// Game language
    /// </summary>
    public class GameLanguage
    {
        /// <summary>
        /// Creates a game language
        /// </summary>
        /// <param name="name">Display name of the langauge</param>
        /// <param name="cultureTag">Culture tag</param>
        public GameLanguage( string name, string cultureTag = "" )
        {
            Name = name;
            CultureTag = cultureTag;
        }

        /// <summary>
        /// Culture tag of the language
        /// </summary>
        public string CultureTag { get;}

        /// <summary>
        /// Display name of the language
        /// </summary>
        public string Name { get; }
    }
}
