using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Markup
{
    // SGF markup properties
    // Web source: http://www.red-bean.com/sgf/properties.html#AR
    public enum MarkupKind
    {
        AreaDim,    // DD
        Arrow,      // AR
        Cross,      // MA
        Circle,     // CR
        Label,      // LB
        Line,       // LN
        Triangle,   // TR
        Square      // SQ
    }
}
