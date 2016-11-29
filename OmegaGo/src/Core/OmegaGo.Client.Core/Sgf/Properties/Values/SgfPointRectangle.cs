using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties.Values
{
    public class SgfPointRectangle : IEnumerable<SgfPoint>
    {
        public SgfPointRectangle( SgfPoint upperLeft, SgfPoint lowerRight)
        {
            
            UpperLeft = upperLeft;
            LowerRight = lowerRight;
        }

        public SgfPoint UpperLeft { get; }

        public SgfPoint LowerRight { get; }

        public IEnumerator<SgfPoint> GetEnumerator()
        {
            return new SgfPointRectangleEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Enumerates a SGF point rectangle
        /// </summary>
        private class SgfPointRectangleEnumerator : IEnumerator<SgfPoint>
        {
            private readonly SgfPointRectangle _pointRectangle;

            public SgfPointRectangleEnumerator( SgfPointRectangle rectangle)
            {
                _pointRectangle = rectangle;
            }

            private SgfPoint _current;

            /// <summary>
            /// Current SGF Point
            /// </summary>
            public SgfPoint Current => _current;

            object IEnumerator.Current => Current;

            /// <summary>
            /// Enumerates next point in the point rectangle
            /// </summary>
            /// <returns>Can move next?</returns>
            public bool MoveNext()
            {
                
            }

            /// <summary>
            /// Resets the enumerator
            /// </summary>
            public void Reset()
            {
                _current = _pointRectangle.UpperLeft;
            }

            public void Dispose()
            {

            }
        }
    }
}
