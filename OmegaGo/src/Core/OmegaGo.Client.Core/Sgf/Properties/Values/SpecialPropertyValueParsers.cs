using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// Parsers for special property values
    /// </summary>
    internal static class SpecialPropertyValueParsers
    {
        /// <summary>
        /// Parses the size property value (SZ)
        /// Can be either nubmer or composed number : number
        /// </summary>
        /// <param name="value">Value to parse</param>
        public static ISgfPropertyValue SizeParser(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Contains(":"))
            {
                //non-square game board
                return SgfComposePropertyValue<int, int>.Parse(value, SgfNumberValue.Parse, SgfNumberValue.Parse);
            }
            //square game board
            return SgfNumberValue.Parse(value);
        }

        /// <summary>
        /// Creates a parser that allows only a certain range of number values
        /// </summary>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns></returns>
        public static SgfPropertyValueParser RangedNumberParser(int min, int max)
        {
            return (value) =>
            {
                var parsedValue = (SgfNumberValue)SgfNumberValue.Parse(value);
                if (parsedValue.Value < min || max < parsedValue.Value)
                {
                    throw new SgfParseException($"Property numeric value outside of the allowed range ({value} - only {min} - {max} allowed).");
                }
                return parsedValue;
            };
        }
    }
}
