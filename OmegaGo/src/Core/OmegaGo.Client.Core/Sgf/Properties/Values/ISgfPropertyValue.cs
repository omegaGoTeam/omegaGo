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
        /// Parses the SGF property value from string
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Parsed property value</returns>
        ISgfPropertyValue Parse(string value);

        /// <summary>
        /// Serializes the SGF property value to string
        /// </summary>
        /// <returns>String</returns>
        string Serialize();
    }
}
