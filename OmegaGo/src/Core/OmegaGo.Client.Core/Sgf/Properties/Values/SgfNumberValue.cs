using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// SGF number property value
    /// </summary>
    public class SgfNumberValue : SgfSimplePropertyValueBase<int>
    {
        /// <summary>
        /// Creates SGF number property value
        /// </summary>
        /// <param name="value">Number value</param>
        public SgfNumberValue(int value) : base(value) { }

        /// <summary>
        /// Number value type
        /// </summary>
        public override SgfValueType ValueType => SgfValueType.Number;

        /// <summary>
        /// Parses SGF nubmer value property
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Parsed SGF number value</returns>
        public static SgfNumberValue Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            int intValue;
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue))
            {
                throw new SgfParseException($"SGF number value could not be parsed from {0}");
            }
            return new SgfNumberValue(intValue);
        }        

        /// <summary>
        /// Serializes SGF number value
        /// </summary>
        /// <returns>Serialized SGF number value</returns>
        public override string Serialize() => Value.ToString("D", CultureInfo.InvariantCulture);
    }
}