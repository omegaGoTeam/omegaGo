using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Sgf.Properties
{
    /// <summary>
    /// Delegate for parsers of property values
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Parsed value</returns>

    public delegate ISgfPropertyValue SgfPropertyValueParser(string value);
}
