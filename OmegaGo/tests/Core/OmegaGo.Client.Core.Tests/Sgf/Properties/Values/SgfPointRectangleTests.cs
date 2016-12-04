using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf.Properties.Values;

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
            Assert.AreEqual(1, rectangle.ToList());
            Assert.AreEqual(single, rectangle.First());
        }
    }
}
