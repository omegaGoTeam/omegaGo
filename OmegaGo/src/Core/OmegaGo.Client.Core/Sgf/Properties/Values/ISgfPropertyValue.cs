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
    public interface ISgfPropertyValue
    {
        SgfValueType ValueType { get; }

        /// <summary>
        /// Serializes the SGF property value to string
        /// </summary>
        /// <returns>String</returns>
        string Serialize();
    }
}
