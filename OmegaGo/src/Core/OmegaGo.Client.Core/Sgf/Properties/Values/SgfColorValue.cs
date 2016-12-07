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
        public SgfColorValue( SgfColor value ) : base( value ) {}

        /// <summary>
        /// Color property value type
        /// </summary>
        public override SgfValueType ValueType => SgfValueType.Color;

        /// <summary>
        /// Parses a SGF Color value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>SGF color instance</returns>
        public static SgfColorValue Parse( string value )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes SGF color instance
        /// </summary>
        /// <returns></returns>
        public override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
