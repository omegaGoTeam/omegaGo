using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// SGF real property value
    /// </summary>
    public class SgfRealValue : SgfSimplePropertyValueBase<decimal>
    {
        /// <summary>
        /// Creates SGF real property value
        /// </summary>
        /// <param name="value"></param>
        public SgfRealValue(decimal value) : base(value) { }

        /// <summary>
        /// Real SGF value
        /// </summary>
        public override SgfValueType ValueType => SgfValueType.Real;

        /// <summary>
        /// Parses SGF real value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Parsed property value</returns>
        public static SgfRealValue Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            for (int i = 0; i < value.Length; i++)
            {
                var character = value[i];
                var isOperator = (character == '+' || character == '-') && i == 0;
                var isNumberPart = char.IsDigit(character) || character == '.';
                var isValid = isOperator || isNumberPart;
                if (!isValid)
                {
                    throw new SgfParseException($"Invalid value format for SGF Real value ({value})");
                }
            }
            decimal decimalValue = 0m;
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimalValue))
            {
                return new SgfRealValue(decimalValue);
            }
            throw new SgfParseException($"SGF Real value could not be parsed from {value}.");
        }

        /// <summary>
        /// Serializes the value
        /// </summary>
        /// <returns>SGF serialized real value</returns>
        public override string Serialize() =>
            Value.ToString(CultureInfo.InvariantCulture);
    }
}
