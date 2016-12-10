using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// SGF Color property value type
    /// </summary>
    public class SgfColorValue : SgfSimplePropertyValueBase<SgfColor>
    {
        /// <summary>
        /// Creates SGF color value
        /// </summary>
        /// <param name="value">Color</param>
        public SgfColorValue(SgfColor value) : base(value)
        {
            if (!Enum.IsDefined(typeof(SgfColor), value))
                throw new ArgumentOutOfRangeException(nameof(value), "SGF Color value must be pre-defined.");
        }

        /// <summary>
        /// Color property value type
        /// </summary>
        public override SgfValueType ValueType => SgfValueType.Color;

        /// <summary>
        /// Parses a SGF Color value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>SGF color instance</returns>
        public static SgfColorValue Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value == "B")
            {
                return new SgfColorValue(SgfColor.Black);
            }
            else if (value == "W")
            {
                return new SgfColorValue(SgfColor.White);
            }
            //color can't be parsed
            throw new SgfParseException($"SGF color value cannot be parsed from '{value}'");
        }

        /// <summary>
        /// Serializes SGF color instance
        /// </summary>
        /// <returns></returns>
        public override string Serialize()
        {
            switch (Value)
            {
                case SgfColor.Black:
                    return "B";
                case SgfColor.White:
                    return "W";
                default:
                    throw new InvalidOperationException("SGF Color value out of supported range.");
            }
        }
    }
}
