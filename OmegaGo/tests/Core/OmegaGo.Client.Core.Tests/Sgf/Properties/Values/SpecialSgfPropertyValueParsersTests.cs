using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Tests.Sgf.Properties.Values
{
    [TestClass]
    public class SpecialSgfPropertyValueParsersTests
    {
        [TestMethod]
        public void SizePropertyParserWorksWithSingleNumber()
        {
            var sizeValue = SpecialSgfPropertyValueParsers.SizeParser("12");
            var sizeNumber = sizeValue as SgfNumberValue;
            Assert.IsNotNull(sizeNumber);
            Assert.AreEqual(12, sizeNumber.Value);
        }

        [TestMethod]
        public void SizePropertyParserWorksWithTwoDimensions()
        {
            var sizeValue = SpecialSgfPropertyValueParsers.SizeParser("12:20");
            var sizeNumber = sizeValue as SgfComposePropertyValue<int, int>;
            Assert.IsNotNull(sizeNumber);
            Assert.AreEqual(12, sizeNumber.LeftValue);
            Assert.AreEqual(20, sizeNumber.RightValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void RangedNumberParserThrowsForInvalidRange()
        {
            SpecialSgfPropertyValueParsers.RangedNumberParser(5, 2);
        }

        [TestMethod]
        public void SingleNumberRangedNumberParserCanParseNumberInRange()
        {
            var parser = SpecialSgfPropertyValueParsers.RangedNumberParser(3, 3);
            var parsed = parser("3") as SgfNumberValue;
            Assert.IsNotNull(parsed);
            Assert.AreEqual(3, parsed.Value);
        }

        [TestMethod]
        public void NumberRangedNumberParserCanParseNumberInRange()
        {
            var parser = SpecialSgfPropertyValueParsers.RangedNumberParser(3, 10);
            var parsed = parser("5") as SgfNumberValue;
            Assert.IsNotNull(parsed);
            Assert.AreEqual(5, parsed.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void NumberRangedNumberParserCantParseNumberBelowRange()
        {
            var parser = SpecialSgfPropertyValueParsers.RangedNumberParser(3, 10);
            parser("0");
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void NumberRangedNumberParserCantParseNumberAboveRange()
        {
            var parser = SpecialSgfPropertyValueParsers.RangedNumberParser(3, 10);
            parser("30");
        }

        [TestMethod]
        public void GameParserCanParseGo()
        {
            var parsed = SpecialSgfPropertyValueParsers.GameParser("1") as SgfNumberValue;
            Assert.IsNotNull(parsed);
            Assert.AreEqual(1, parsed.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void GameParserCantParseOtherValueThanGo()
        {
            SpecialSgfPropertyValueParsers.GameParser("2");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GameParserCantParseNull()
        {
            SpecialSgfPropertyValueParsers.GameParser(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FigureParserCantParseNull()
        {
            SpecialSgfPropertyValueParsers.FigureParser(null);
        }

        [TestMethod]
        public void FigureParserReturnsNullForNoneValue()
        {
            var none = SpecialSgfPropertyValueParsers.FigureParser("");
            Assert.IsNull(none);
        }

        [TestMethod]
        public void FigureParserCanReadPointCompose()
        {
            var parsed = SpecialSgfPropertyValueParsers.FigureParser("1:Hello") as SgfComposePropertyValue<int, string>;
            Assert.IsNotNull(parsed);
            Assert.AreEqual(1, parsed.LeftValue);
            Assert.AreEqual("Hello", parsed.LeftValue);
        }
    }
}
