using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Sgf.Parsing
{
	/// <summary>
    /// This part contains the definition for the list of known properties
    /// </summary>
    internal static partial class SgfPropertyValuesConverter
    {
        /// <summary>
        /// Defined parsing methods for known SGF properties
        /// </summary>
        private static readonly Dictionary<string, KnownPropertyValueParser> KnownPropertyValueParsers =
            new List<KnownPropertyValueParser>()
            {
                new KnownPropertyValueParser( "B", SgfPointValue.Parse, false ),
                new KnownPropertyValueParser( "w", SgfPointValue.Parse, false )
            }.ToDictionary(i => i.Identifier, i => i);
    }
}
