﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Tests.Sgf.Properties.Values.ValueTypes
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

        [TestMethod]
        public void FullSizePointRectangleHasTheRightNumberOfPoints()
        {
            var upperLeft = SgfPoint.Parse("aa");
            var lowerRight = SgfPoint.Parse("ZZ");
            var rectangle = new SgfPointRectangle(upperLeft, lowerRight);            
            Assert.AreEqual(2704, rectangle.Count());
        }

        [TestMethod]
        public void ToStringSerializesSinglePointPointRectangleProperly()
        {
            var upperLeft = SgfPoint.Parse("aa");
            var lowerRight = SgfPoint.Parse("aa");
            var rectangle = new SgfPointRectangle(upperLeft, lowerRight);
            Assert.AreEqual("aa", rectangle.ToString());
        }

        [TestMethod]
        public void ToStringSerializesRectangularPointRectangleProperly()
        {
            var upperLeft = SgfPoint.Parse("aa");
            var lowerRight = SgfPoint.Parse("ZZ");
            var rectangle = new SgfPointRectangle(upperLeft, lowerRight);
            Assert.AreEqual("aa:ZZ", rectangle.ToString());
        }
    }
}
