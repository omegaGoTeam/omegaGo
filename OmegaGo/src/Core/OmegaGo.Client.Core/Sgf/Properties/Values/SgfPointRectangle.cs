using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    /// <summary>
    /// Represents a rectangle of points
    /// </summary>
    public class SgfPointRectangle : IEnumerable<SgfPoint>
    {
        public SgfPointRectangle(SgfPoint upperLeft, SgfPoint lowerRight)
        {

            UpperLeft = upperLeft;
            LowerRight = lowerRight;
        }

        public SgfPoint UpperLeft { get; }

        public SgfPoint LowerRight { get; }

        /// <summary>
        /// Enumerates the points inside of the point rectangle
        /// </summary>
        /// <returns></returns>
        public IEnumerator<SgfPoint> GetEnumerator()
        {
            for (var row = UpperLeft.Row; row <= LowerRight.Row; row++)
            {
                for (var column = UpperLeft.Column; column <= LowerRight.Column; column++)
                {
                    yield return new SgfPoint(column, row);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }        
    }
}
