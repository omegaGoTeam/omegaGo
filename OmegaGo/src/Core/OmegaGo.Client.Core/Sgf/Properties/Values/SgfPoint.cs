using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// SGF point
    /// </summary>
    public struct SgfPoint
    {
        /// <summary>
        /// Creates a SGF Point
        /// </summary>
        /// <param name="column">Column</param>
        /// <param name="row">Row</param>
        public SgfPoint( char column, char row ) : this()
        {
            Column = column;
            Row = row;
        }

        /// <summary>
        /// Column
        /// </summary>
        public char Column { get; }

        /// <summary>
        /// Row
        /// </summary>
        public char Row { get; }
    }
}
