﻿using OmegaGo.Core.Sgf.Properties.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties
{
    /// <summary>
    /// Class that facilitates the conversion of known SGF property values
    /// </summary>
    internal static class SgfPropertiesValuesConverter
    {
        /// <summary>
        /// Returns the parsed values for a given property
        /// </summary>
        /// <param name="propertyIdentifier">Identifier of the property</param>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public static ISgfPropertyValue GetValue(string propertyIdentifier, string value)
        {
            //is the property known?
            if (KnownPropertyParsers.ContainsKey(propertyIdentifier))
            {
                return KnownPropertyParsers[propertyIdentifier](value);
            }
            //return as unknown property
            return SgfUnknownPropertyValue.Parse(value);
        }

        private static readonly Dictionary<string, Func<string, ISgfPropertyValue>> KnownPropertyParsers =
            new Dictionary<string, Func<string, ISgfPropertyValue>>()
            {

            };
    }
}
