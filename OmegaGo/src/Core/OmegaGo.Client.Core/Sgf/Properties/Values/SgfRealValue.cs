using System;
using System.Collections.Generic;
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
        public SgfRealValue( decimal value ) : base( value ) {}

        /// <summary>
        /// Real SGF value
        /// </summary>
        public override SgfValueType ValueType => SgfValueType.Real;

        /// <summary>
        /// Parses SGF real value
        /// </summary>
        /// <param name="value">Value to parse</param>
        /// <returns>Parsed property value</returns>
        public static ISgfPropertyValue Parse( string value )
        {
            throw new NotImplementedException();   
        }

        /// <summary>
        /// Serializes the value
        /// </summary>
        /// <returns>SGF serialized real value</returns>
        public override string Serialize()
        {
            throw new NotImplementedException();
        }
    }
}
