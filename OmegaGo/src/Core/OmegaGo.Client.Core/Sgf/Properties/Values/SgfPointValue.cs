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
    public class SgfPointValue : SgfValue<IEnumerable<SgfPoint>>
    {
        public SgfPointValue( IEnumerable<SgfPoint> points ) : base( points )
        {
            
        } 
    }
}
