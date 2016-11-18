using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties
{
    /// <summary>
    /// Types of SGF properties
    /// </summary>
    public enum SgfPropertyType
    {
        Move,
        Setup,
        Root,
        GameInfo,
        NoType,
        Deprecated,
        Unknown,
        Invalid,       
    }
}
