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
    public struct SgfPoint : IComparable<SgfPoint>
    {
        /// <summary>
        /// Creates a SGF Point
        /// </summary>
        /// <param name="column">Column</param>
        /// <param name="row">Row</param>
        public SgfPoint(int column, int row)
        {
            Column = column;
            Row = row;
        }

        /// <summary>
        /// Column
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Row
        /// </summary>
        public int Row { get; }

        public static bool operator <(SgfPoint left, SgfPoint right) => left.CompareTo(right) < 0;

        public static bool operator <=(SgfPoint left, SgfPoint right) => left.CompareTo(right) <= 0;

        public static bool operator >(SgfPoint left, SgfPoint right) => left.CompareTo(right) > 0;

        public static bool operator >=(SgfPoint left, SgfPoint right) => left.CompareTo(right) >= 0;


        /// <summary>
        /// Compares two SGF points
        /// </summary>
        /// <param name="other">SGF point to compare</param>
        /// <returns>The ordering of SGF points</returns>
        public int CompareTo(SgfPoint other)
        {
            if (Column <= other.Column && Row <= other.Row)
            {
                if (Column == other.Column && Row == other.Row)
                {
                    return 0;
                }
                return -1;
            }
            return 1;
        }
    }
}
