using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    public class SgfDoubleValue : SgfSimplePropertyValueBase<SgfDouble>
    {
        public SgfDoubleValue(SgfDouble value) : base(value)
        {
        }

        public override SgfValueType ValueType => SgfValueType.Double;

        /// <summary>
        /// Parses a value to SGF double property value
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>SGF double property value instance</returns>
        public static SgfDoubleValue Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            SgfDouble doubleValue;
            if (Enum.TryParse(value, out doubleValue))
            {
                if (Enum.IsDefined(typeof(SgfDouble), doubleValue))
                {
                    return new SgfDoubleValue(doubleValue);
                }
            }
            throw new SgfParseException($"SGF double value cannot be parsed properly: {value}");
        }

        /// <summary>
        /// Serializes SgfDouble as integer
        /// </summary>
        /// <returns>SGF double as integer string</returns>
        public override string Serialize() => Value.ToString("D");
    }
}
