using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// SGF Point property value
    /// </summary>
    public class SgfPointValue : SgfValue
    {
        public SgfPointValue( IEnumerable<SgfPoint> points ) : base( points )
        {
            
        }

        /// <summary>
        /// Implicit conversion from value instance to value
        /// </summary>
        /// <param name="valueInstance"></param>
        public static implicit operator SgfPoint( SgfPointValue valueInstance )
        {
            return valueInstance.Value;
        }

        public static implicit operator SgfPointValue( SgfPoint value )
        {
            return new SgfPointValue( value );
        }

        private SgfPoint Value { get; set; }
    }
}
