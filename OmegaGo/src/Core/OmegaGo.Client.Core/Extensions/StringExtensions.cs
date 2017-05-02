using System;
using System.Globalization;

namespace OmegaGo.Core.Extensions
{
    /// <summary>
    /// Extensions for strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Parses string to int
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>String converted to integer</returns>
        public static int AsInteger(this string text)
        {
            return int.Parse(text);
        }

        /// <summary>
        /// Parses string to float
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="formatProvider">Format provider</param>
        /// <returns>String converted to float</returns>
        public static float AsFloat(this string text, IFormatProvider formatProvider = null)
        {            
            return float.Parse(text, NumberStyles.Float, formatProvider ?? CultureInfo.InvariantCulture);
        }
    }
}
