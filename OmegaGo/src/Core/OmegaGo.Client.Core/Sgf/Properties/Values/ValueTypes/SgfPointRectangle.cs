using System;
using System.Collections;
using System.Collections.Generic;
using OmegaGo.Core.Sgf.Helpers;

namespace OmegaGo.Core.Sgf.Properties.Values.ValueTypes
{
    /// <summary>
    /// Represents a rectangle of points
    /// </summary>
    public struct SgfPointRectangle : IEquatable<SgfPointRectangle>, IEnumerable<SgfPoint>
    {
        /// <summary>
        /// Creates a point rectangle
        /// </summary>
        /// <param name="upperLeft">Upper-left corner of the rectangle</param>
        /// <param name="lowerRight">Lower-right corner of the rectangle</param>
        public SgfPointRectangle(SgfPoint upperLeft, SgfPoint lowerRight)
        {
            if (upperLeft.Row > lowerRight.Row || upperLeft.Column > lowerRight.Column)
                throw new ArgumentOutOfRangeException($"SGF point rectangle points must represent a rectangle: {upperLeft}:{lowerRight}");
            UpperLeft = upperLeft;
            LowerRight = lowerRight;
        }

        /// <summary>
        /// Creates a single point rectangle
        /// </summary>
        /// <param name="singlePoint"></param>
        public SgfPointRectangle(SgfPoint singlePoint) : this(singlePoint, singlePoint)
        {
        }

        /// <summary>
        /// Pass
        /// </summary>
        public static SgfPointRectangle Pass { get; } = new SgfPointRectangle(SgfPoint.Pass);

        public SgfPoint UpperLeft { get; }

        public SgfPoint LowerRight { get; }

        /// <summary>
        /// Checks if the point rectangle represents a single point
        /// </summary>
        public bool IsSinglePoint => UpperLeft == LowerRight;

        /// <summary>
        /// Checks if the point rectangle represents a Pass move
        /// </summary>
        public bool IsPass => UpperLeft.Row == -1;

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

        public static bool operator ==(SgfPointRectangle left, SgfPointRectangle right) => left.Equals(right);

        public static bool operator !=(SgfPointRectangle left, SgfPointRectangle right) => !(left == right);

        public static SgfPointRectangle[] CompressPoints(params SgfPoint[] points)
        {
            return new SgfPointCompressor(points).CompressPoints();
        }

        /// <summary>
        /// Checks for equality of two SGF point rectangles
        /// </summary>
        /// <param name="other">Second point rectangle</param>
        /// <returns>Are they equal?</returns>
        public bool Equals(SgfPointRectangle other) =>
            UpperLeft == other.UpperLeft && LowerRight == other.LowerRight;

        /// <summary>
        /// Checks for equality of two instances
        /// </summary>
        /// <param name="obj">Second instance</param>
        /// <returns>Are they equal?</returns>
        public override bool Equals(object obj)
        {
            var second = obj as SgfPointRectangle?;
            if (second != null)
            {
                return Equals(second.Value);
            }
            return false;
        }

        /// <summary>
        /// Gets a hash code of the instance
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode() =>
            UpperLeft.GetHashCode() * 13 + LowerRight.GetHashCode();

        /// <summary>
        /// Serializes the SGF point rectangle
        /// </summary>
        /// <returns>Serialized SGF point rectangle value representation</returns>
        public override string ToString()
        {
            if (UpperLeft == LowerRight)
            {
                //single point
                return UpperLeft.ToString();
            }
            return $"{UpperLeft}:{LowerRight}";
        }
    }
}
