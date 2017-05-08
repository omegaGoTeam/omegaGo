using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf.Helpers;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Tests.Sgf.Helpers
{
    [TestClass]
    public class SgfPointCompressorTests
    {
        [TestMethod]
        public void SinglePointCompressionResultsInSinglePointRectangle()
        {
            SgfPointCompressor compressor = new SgfPointCompressor(new[] { new SgfPoint(23, 12) });
            var pointRectangles = compressor.CompressPoints();
            Assert.AreEqual(1, pointRectangles.Length);
            Assert.AreEqual(new SgfPoint(23, 12), pointRectangles.First().UpperLeft);
            Assert.AreEqual(new SgfPoint(23, 12), pointRectangles.First().LowerRight);
        }

        [TestMethod]
        public void SingleLineIsCompressedIntoSinglePointRectangle()
        {
            SgfPointCompressor compressor = new SgfPointCompressor(new[]
            {
                new SgfPoint(23, 12), new SgfPoint(24, 12), new SgfPoint(25, 12)
            });
            var pointRectangles = compressor.CompressPoints();
            Assert.AreEqual(1, pointRectangles.Length);
            Assert.AreEqual(new SgfPoint(23, 12), pointRectangles.First().UpperLeft);
            Assert.AreEqual(new SgfPoint(25, 12), pointRectangles.First().LowerRight);
        }

        [TestMethod]
        public void LargerRectangleIsCompressedIntoSinglePointRectangle()
        {
            SgfPointCompressor compressor = new SgfPointCompressor(new[]
            {
                new SgfPoint(23, 12), new SgfPoint(24, 12), new SgfPoint(25, 12),
                new SgfPoint(23, 13), new SgfPoint(24, 13), new SgfPoint(25, 13)
            });
            var pointRectangles = compressor.CompressPoints();
            Assert.AreEqual(1, pointRectangles.Length);
            Assert.AreEqual(new SgfPoint(23, 12), pointRectangles.First().UpperLeft);
            Assert.AreEqual(new SgfPoint(25, 13), pointRectangles.First().LowerRight);
        }

        [TestMethod]
        public void TwoComponentsResultInTwoPointRectangles()
        {
            SgfPointCompressor compressor = new SgfPointCompressor(new[]
            {
                new SgfPoint(23, 12), new SgfPoint(25, 12),
                new SgfPoint(23, 13), new SgfPoint(25, 13)
            });
            var pointRectangles = compressor.CompressPoints();
            Assert.AreEqual(2, pointRectangles.Length);
            Assert.AreEqual(new SgfPoint(23, 12), pointRectangles.First().UpperLeft);
            Assert.AreEqual(new SgfPoint(23, 13), pointRectangles.First().LowerRight);
            Assert.AreEqual(new SgfPoint(25, 12), pointRectangles.Last().UpperLeft);
            Assert.AreEqual(new SgfPoint(25, 13), pointRectangles.Last().LowerRight);
        }

        [TestMethod]
        public void PointRectanglePointsCompressionResultsInOriginalPointRectangle()
        {
            SgfPointCompressor compressor = new SgfPointCompressor(
                new SgfPointRectangle(new SgfPoint(0, 0), new SgfPoint(52, 52)).ToArray()
            );
            var pointRectangles = compressor.CompressPoints();
            Assert.AreEqual(1, pointRectangles.Length);
            Assert.AreEqual(new SgfPointRectangle(new SgfPoint(0, 0), new SgfPoint(52, 52)),
                pointRectangles.First());
        }

        [TestMethod]
        public void PointRectanglePointsCompressionWithMissingCornerResultsInTwoPointRectangles()
        {
            SgfPointCompressor compressor = new SgfPointCompressor(
                new SgfPointRectangle(new SgfPoint(0, 0), new SgfPoint(52, 52)).Except(new[] { new SgfPoint(52, 52) }).ToArray()
            );
            var pointRectangles = compressor.CompressPoints();
            Assert.AreEqual(2, pointRectangles.Length);
        }

        [TestMethod]
        public void ComplexPointRectanglePointsCanBeCompressed()
        {
            var rectangles = new SgfPointRectangle[]
            {
                new SgfPointRectangle(SgfPoint.Parse("jd"), SgfPoint.Parse("jg")),
                new SgfPointRectangle(SgfPoint.Parse("kn"), SgfPoint.Parse("lq")),
                new SgfPointRectangle(SgfPoint.Parse("pn"), SgfPoint.Parse("pq")),
            };
            var allPoints = rectangles.SelectMany(pr => pr.ToList()).ToArray();
            SgfPointCompressor compressor = new SgfPointCompressor(allPoints);
            var pointRectangles = compressor.CompressPoints();
            var decompressedPoints = pointRectangles.SelectMany(pr => pr.ToList());
            Assert.AreEqual(0, allPoints.Except(decompressedPoints).Count());
            Assert.AreEqual(0, pointRectangles.Except(rectangles).Count());
        }
    }
}
