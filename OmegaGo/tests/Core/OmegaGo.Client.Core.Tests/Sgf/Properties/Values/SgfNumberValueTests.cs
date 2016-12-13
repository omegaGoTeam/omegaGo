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
    public class SgfNumberValueTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParsingNullNumberValueThrows()
        {
            SgfNumberValue.Parse( null );
        }

        [TestMethod]
        public void SimpleNumericValueIsProperlyParsed()
        {
            var propertyValue = SgfNumberValue.Parse("1");
            Assert.AreEqual(1, propertyValue.Value);
        }

        [TestMethod]
        public void PositiveNumericValueIsProperlyParsed()
        {
            var propertyValue = SgfNumberValue.Parse("+211");
            Assert.AreEqual(211, propertyValue.Value);
        }

        [TestMethod]
        public void NegativeNumericValueIsProperlyParsed()
        {
            var propertyValue = SgfNumberValue.Parse("-24351");
            Assert.AreEqual(-24351, propertyValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void EmptyNumericValueParseThrows()
        {
            var propertyValue = SgfNumberValue.Parse(string.Empty);            
        }

        [TestMethod]
        public void NumericValueWithLeadingZerosIsProperlyParsed()
        {
            var propertyValue = SgfNumberValue.Parse("-0004351");
            Assert.AreEqual(-4351, propertyValue.Value);
        }

        [TestMethod]
        public void NumericValueIsProperlySerialized()
        {
            var propertyValue = new SgfNumberValue(123);
            Assert.AreEqual("123", propertyValue.Serialize());
        }

        [TestMethod]
        public void NegativeNumericValueIsProperlySerialized()
        {
            var propertyValue = new SgfNumberValue(-2353);
            Assert.AreEqual("-2353", propertyValue.Serialize());
        }
    }
}
