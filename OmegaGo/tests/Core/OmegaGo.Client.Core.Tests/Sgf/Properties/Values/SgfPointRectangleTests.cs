using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf.Properties.Values;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Tests.Sgf.Properties.Values
{
    [TestClass]
    public class SgfPointRectangleTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SgfPointRectangleDoesNotAcceptNonRectangleAreas()
        {
            var point = new SgfPointRectangle(new SgfPoint(1, 2), new SgfPoint(0, 0));
        }

        [TestMethod]
        public void SinglePointRectangleIsProperlyCreated()
        {
            var single = new SgfPoint(12, 2);
            var rectangle = new SgfPointRectangle(single);
            Assert.AreEqual(single, rectangle.UpperLeft);
            Assert.AreEqual(single, rectangle.LowerRight);
            Assert.AreEqual(1, rectangle.Count());
            Assert.AreEqual(single, rectangle.First());
        }

        [TestMethod]
        public void MultiplePointsAreProperlyEnumerated()
        {
            var upperLeft = new SgfPoint(10, 5);
            var lowerRight = new SgfPoint(11, 6);
            var rectangle = new SgfPointRectangle(upperLeft, lowerRight);
            var enumerated = rectangle.ToList();
            Assert.AreEqual(4, enumerated.Count);
            Assert.AreEqual(upperLeft, enumerated.First());
            Assert.AreEqual(new SgfPoint(11, 5), enumerated[1]);
            Assert.AreEqual(new SgfPoint(10, 6), enumerated[2]);
            Assert.AreEqual(lowerRight, enumerated.Last());
        }        
    }
}
