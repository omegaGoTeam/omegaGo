using System;
using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Sgf.Parsing
{
    /// <summary>
    /// Class that facilitates the conversion of known SGF property values
    /// </summary>
    internal static partial class SgfPropertyValuesConverter
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
            if (KnownPropertyValueParsers.ContainsKey(propertyIdentifier))
            {
                var propertyParserDefinition = KnownPropertyValueParsers[propertyIdentifier];

                return ParseValues(values, propertyParserDefinition.Parser);                
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
        private static IEnumerable<ISgfPropertyValue> ParseValues(string[] values, PropertyValueParser parser)
            => values.Select( value => parser( value ) );        

        private delegate ISgfPropertyValue PropertyValueParser(string value);

        /// <summary>
        /// Specifies the number of values a property is allowed to have
        /// </summary>
        private enum ValueMultiplicity
        {
            Single,
            List,
            EList,
            None
        }

        /// <summary>
        /// Defines the value parser for a known property
        /// </summary>
        private class KnownPropertyValueParser
        {
            public KnownPropertyValueParser(string identifier, PropertyValueParser parser, ValueMultiplicity valueMultiplicity)
            {
                Identifier = identifier;
                Parser = parser;
                ValueMultiplicity = valueMultiplicity;
            }

            public string Identifier { get; }

            public PropertyValueParser Parser { get; }

            public ValueMultiplicity ValueMultiplicity { get; }
        }
    }
}
