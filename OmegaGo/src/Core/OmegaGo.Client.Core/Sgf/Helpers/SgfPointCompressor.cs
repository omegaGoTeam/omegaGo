using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Sgf.Helpers
{
    /// <summary>
    /// Compresses SGF points to SGF point rectangles
    /// </summary>
    internal class SgfPointCompressor
    {
        /// <summary>
        /// Represents a rectangle expansion direction
        /// </summary>
        private struct ExpandDirection
        {
            public ExpandDirection(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

        /// <summary>
        /// Directions in which rectangles may expand
        /// </summary>
        private static readonly ExpandDirection[] ExpandDirections = new ExpandDirection[]
        {
            new ExpandDirection(-1, 0),
            new ExpandDirection(0, -1),
            new ExpandDirection(1, 0),
            new ExpandDirection(0, 1),
        };

        private readonly HashSet<SgfPoint> _unusedPoints;
        private readonly List<SgfPointRectangle> _compressedResults = new List<SgfPointRectangle>();

        public SgfPointCompressor(SgfPoint[] points)
        {
            _unusedPoints = new HashSet<SgfPoint>(points);
        }

        public SgfPointRectangle[] CompressPoints()
        {
            while (_unusedPoints.Count != 0)
            {
                _compressedResults.Add(ExpandFromPoint(_unusedPoints.First()));
            }
            return _compressedResults.ToArray();
        }

        private SgfPointRectangle ExpandFromPoint(SgfPoint point)
        {
            var topEdge = point.Row;
            var bottomEdge = point.Row;
            var leftEdge = point.Column;
            var rightEdge = point.Column;
            _unusedPoints.Remove(point);
            Queue<ExpandDirection> availableExpandDirections = new Queue<ExpandDirection>(ExpandDirections);
            while (availableExpandDirections.Any())
            {
                var direction = availableExpandDirections.Dequeue();
                bool expanded = false;

                //try to expand
                if (direction.Y != 0)
                {
                    var checkedRow = direction.Y < 0 ?
                        topEdge - 1 :
                        bottomEdge + 1;

                    if (checkedRow >= 0 && checkedRow <= 52)
                    {
                        var expandable = true;
                        //check direction for expansion
                        for (int checkedColumn = leftEdge; checkedColumn <= rightEdge; checkedColumn++)
                        {
                            if (!_unusedPoints.Contains(new SgfPoint(checkedColumn, checkedRow)))
                            {
                                expandable = false;
                                break;
                            }
                        }
                        if (expandable)
                        {
                            //mark points as used
                            for (int checkedColumn = leftEdge; checkedColumn <= rightEdge; checkedColumn++)
                            {
                                _unusedPoints.Remove(new SgfPoint(checkedColumn, checkedRow));
                            }
                            //expand boundary
                            if (direction.Y < 0)
                            {
                                topEdge--;
                            }
                            else if (direction.Y > 0)
                            {
                                bottomEdge++;
                            }
                            expanded = true;
                        }
                    }
                }
                else if (direction.X != 0)
                {
                    var checkedColumn = direction.X < 0 ?
                        leftEdge - 1 :
                        rightEdge + 1;

                    if (checkedColumn >= 0 && checkedColumn <= 52)
                    {
                        var expandable = true;
                        //check direction for expansion
                        for (int checkedRow = topEdge; checkedRow <= bottomEdge; checkedRow++)
                        {
                            if (!_unusedPoints.Contains(new SgfPoint(checkedColumn, checkedRow)))
                            {
                                expandable = false;
                                break;
                            }
                        }
                        if (expandable)
                        {
                            //mark points as used
                            for (int checkedRow = topEdge; checkedRow <= bottomEdge; checkedRow++)
                            {
                                _unusedPoints.Remove(new SgfPoint(checkedColumn, checkedRow));
                            }
                            //expand boundary
                            if (direction.X < 0)
                            {
                                leftEdge--;
                            }
                            else if (direction.X > 0)
                            {
                                rightEdge++;
                            }
                            expanded = true;
                        }
                    }
                }

                if (expanded)
                {
                    availableExpandDirections.Enqueue(direction);
                }
            }
            return new SgfPointRectangle(new SgfPoint(leftEdge, topEdge), new SgfPoint(rightEdge, bottomEdge));
        }
    }
}