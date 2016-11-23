using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// SGF property value
    /// </summary>
    internal abstract class SgfValue
    {
        protected SgfValue( SgfValueType type )
        {
            Type = type;
        }

        public SgfValueType Type { get; }
    }
}