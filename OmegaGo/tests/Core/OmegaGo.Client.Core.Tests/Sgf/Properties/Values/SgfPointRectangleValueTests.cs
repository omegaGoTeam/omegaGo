using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Properties.Values;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Tests.Sgf.Properties.Values
{
    [TestClass]
    public class SgfPointRectangleValueTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullParsingThrows()
        {
            SgfPointRectangleValue.Parse(null);
        }

        [TestMethod]
        public void ValidPointRectangleValueCanBeParsed()
        {
            var propertyValue = SgfPointRectangleValue.Parse("aa:ZZ");
            Assert.AreEqual(new SgfPoint(0, 0), propertyValue.Value.UpperLeft);
            Assert.AreEqual(new SgfPoint(51, 51), propertyValue.Value.LowerRight);
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void InvalidPointRectangleValueCantBeParsed()
        {
            SgfPointRectangleValue.Parse("aa:Z");
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void InvalidPointRectangleValueCantBeParsedWithoutColon()
        {
            SgfPointRectangleValue.Parse("aaZZ");
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void SinglePointRectangleValueCantBeParsedWithColon()
        {
            SgfPointRectangleValue.Parse("aa:aa");
        }

        [TestMethod]
        public void SinglePointRectangleValueCanBeParsedWithoutColon()
        {
            var propertyValue = SgfPointRectangleValue.Parse("aa");
            Assert.AreEqual(new SgfPoint(0, 0), propertyValue.Value.UpperLeft);
            Assert.AreEqual(new SgfPoint(0, 0), propertyValue.Value.LowerRight);
        }

        [TestMethod]
        public void SameRowRectangleCanBeParsed()
        {
            var propertyValue = SgfPointRectangleValue.Parse("aa:ba");
            Assert.AreEqual(new SgfPoint(0, 0), propertyValue.Value.UpperLeft);
            Assert.AreEqual(new SgfPoint(1, 0), propertyValue.Value.LowerRight);
        }

        [TestMethod]
        public void SameColumnRectangleCanBeParsed()
        {
            var propertyValue = SgfPointRectangleValue.Parse("aa:ab");
            Assert.AreEqual(new SgfPoint(0, 0), propertyValue.Value.UpperLeft);
            Assert.AreEqual(new SgfPoint(0, 1), propertyValue.Value.LowerRight);
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void BackwardsRectangleCantBeParsed()
        {
            SgfPointRectangleValue.Parse("cc:aa");
        }

        [TestMethod]
        public void SimpleOnePointPointRectangleCanBeSerialized()
        {
            var propertyValue = new SgfPointRectangleValue( new SgfPointRectangle(new SgfPoint(0,0)));
            Assert.AreEqual("aa", propertyValue.Serialize());
        }

        [TestMethod]
        public void RectangularPointRectangleCanBeSerialized()
        {
            var propertyValue = new SgfPointRectangleValue(new SgfPointRectangle(new SgfPoint(0, 0), new SgfPoint(51, 51)));
            Assert.AreEqual("aa:ZZ", propertyValue.Serialize());
        }
    }
}
