using System;
using System.Globalization;

namespace OmegaGo.Core.Extensions
{
    /// <summary>
    /// Extensions for strings
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Parses string to int
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns></returns>
        public static int AsInteger(this string text)
        {
            return int.Parse(text);
        }

        /// <summary>
        /// Parses string to float
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="formatProvider">Format provider</param>
        /// <returns></returns>
        public static float AsFloat(this string text, IFormatProvider formatProvider = null)
        {            
            return float.Parse(text, NumberStyles.Float, formatProvider ?? CultureInfo.InvariantCulture);
        }
    }
}
