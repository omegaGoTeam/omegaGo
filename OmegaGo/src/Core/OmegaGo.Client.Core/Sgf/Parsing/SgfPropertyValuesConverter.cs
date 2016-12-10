using System;
using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Sgf.Properties;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Sgf.Parsing
{
    /// <summary>
    /// Class that facilitates the conversion of known SGF property values
    /// </summary>
    internal static class SgfPropertyValuesConverter
    {
        /// <summary>
        /// Returns the parsed values for a given property
        /// </summary>
        /// <param name="propertyIdentifier">Identifier of the property</param>
        /// <param name="values">Value to convert</param>
        /// <returns>Converted value</returns>
        public static IEnumerable<ISgfPropertyValue> GetValues(string propertyIdentifier, params string[] values)
        {
            if (propertyIdentifier == null) throw new ArgumentNullException(nameof(propertyIdentifier));
            if (values == null) throw new ArgumentNullException(nameof(values));

            //is the property known?
            var property = SgfKnownProperties.Get(propertyIdentifier);
            if ( property != null)
            { 
                return ParseValues(values, property.Parser);
            }
            //return as unknown property
            return ParseValues(values, SgfUnknownValue.Parse);
        }

        /// <summary>
        /// Parses given values using a parser
        /// </summary>
        /// <param name="values">Values to parse</param>
        /// <param name="parser">Parser to use</param>
        /// <returns>Parsed SGF property values</returns>
        private static IEnumerable<ISgfPropertyValue> ParseValues(string[] values, SgfPropertyValueParser parser)
            => values.Select(value => parser(value));
    }
}
