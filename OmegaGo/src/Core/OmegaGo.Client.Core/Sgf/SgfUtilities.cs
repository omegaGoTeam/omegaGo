using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Utitlities useful throughout SGF handling
    /// </summary>
    internal static class SgfUtilities
    {
        /// <summary>
        /// Escapes a given string
        /// </summary>
        /// <param name="text">String to escape</param>
        /// <returns>Escaped string</returns>
        public static string EscapeText( this string text )
        {
            return text;
        }

        /// <summary>
        /// Unescapes a given string
        /// </summary>
        /// <param name="text">String to unescape</param>
        /// <returns>Unescaped string</returns>
        public static string UnescapeText( this string text)
        {
           return text;
        }
    }
}
