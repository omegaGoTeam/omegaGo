using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Parsing;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// Parsers for special property values
    /// </summary>
    internal static class SpecialSgfPropertyValueParsers
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
            if ( max < min ) throw new ArgumentOutOfRangeException(nameof(max),$"Specified range for SGF ranged number parser is invalid ({min}-{max})");
            return (value) =>
            {
                var parsedValue = SgfNumberValue.Parse(value);
                if (parsedValue.Value < min || max < parsedValue.Value)
                {
                    throw new SgfParseException($"Property numeric value outside of the allowed range ({value} - only {min} - {max} allowed).");
                }
                return parsedValue;
            };
        }

        /// <summary>
        /// Parses the SGF Game property
        /// This library supports only Go = 1
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Parsed property</returns>
        public static ISgfPropertyValue GameParser(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value != "1") throw new SgfParseException("This library supports parsing of the Go game SGF files only.");
            //return nubmer 1 as Go game
            return new SgfNumberValue(1);
        }

        /// <summary>
        /// Parses the SGF Figure property
        /// Allows either empty or compose nubmer : simple text
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Parsed SGF Figure or null if none value</returns>
        public static ISgfPropertyValue FigureParser( string value )
        {
            if ( value == null ) throw new ArgumentNullException( nameof( value ) );
            if ( value == "" )
            {
                //none value
                return null;
            }
            //parse compose
            return SgfComposePropertyValue<int, string>.Parse( value, SgfNumberValue.Parse, SgfSimpleTextValue.Parse );
        }
    }
}
