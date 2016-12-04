﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// Unknown SGF property value
    /// </summary>
    public class SgfUnknownPropertyValue : SgfSimplePropertyValueBase<string>
    {
        /// <summary>
        /// Stores simple string value
        /// </summary>
        /// <param name="value">String value</param>
        public SgfUnknownPropertyValue(string value) : base(value)
        {
        }

        /// <summary>
        /// No parsing performed, just wrap the value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Unknown property value instance</returns>
        public static ISgfPropertyValue Parse(string value) => new SgfUnknownPropertyValue(value);

        /// <summary>
        /// Value type
        /// </summary>
        public override SgfValueType ValueType => SgfValueType.Unknown;

        /// <summary>
        /// Serializes the unknown property value
        /// </summary>
        /// <returns>Value itself</returns>
        public override string Serialize() => Value;
    }
}
