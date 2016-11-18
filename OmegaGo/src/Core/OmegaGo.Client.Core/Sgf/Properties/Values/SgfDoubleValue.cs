using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// SGF double property value
    /// </summary>
    public class SgfDoubleValue : SgfValue<SgfDouble>
    {
        public SgfDoubleValue( SgfDouble value ) : base( value )
        {

        }

        /// <summary>
        /// Serializes value into string
        /// </summary>
        /// <returns>Serialized representation of the property value</returns>
        public override string ToString() => $"[{Value:D}]";
    }
}
