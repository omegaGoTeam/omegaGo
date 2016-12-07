using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties
{
    /// <summary>
    /// Specifies the number of values a property is allowed to have
    /// </summary>
    internal enum SgfValueMultiplicity
    {
        None = 0,
        Single = 1,
        EList = 2,
        List = 3
    }
}
