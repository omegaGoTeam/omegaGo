using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents the result of SGF parsing
    /// Informs the caller about possible warnings during the parsing
    /// </summary>
    internal class SgfParseResult
    {
        /// <summary>
        /// Creates the parse result
        /// </summary>
        /// <param name="status">Status of parsing</param>
        /// <param name="root">Root of the parsed SGF tree</param>
        public SgfParseResult(SgfParseStatus status, SgfRoot root )
        {
            if (root == null ) throw new ArgumentNullException(nameof(root), "Root has to be supplied when parsing did not fail");
            Status = status;
            Root = root;
        }

        /// <summary>
        /// Status of the parsing
        /// </summary>
        public SgfParseStatus Status { get; }

        /// <summary>
        /// Root of the parsed SGF tree
        /// </summary>
        public SgfRoot Root { get; }
    }
}
