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
    public class SgfDoubleValueTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ParsingDoubleValueThrowsWithNull()
        {
            var value = SgfDoubleValue.Parse(null);
        }

        [TestMethod]
        public void ParsingValidDoubleValueReturnsDoubleValue()
        {
            var value = SgfDoubleValue.Parse("1");
            Assert.IsTrue(value is SgfDoubleValue);            
        }

        [TestMethod]
        public void ParsingSimpleDoubleWorks()
        {
            var propertyValue = (SgfDoubleValue)SgfDoubleValue.Parse("1");
            Assert.AreEqual(SgfDouble.Simple, propertyValue.Value);
        }

        [TestMethod]
        public void ParsingEmphasizedDoubleWorks()
        {
            var propertyValue = (SgfDoubleValue)SgfDoubleValue.Parse("2");
            Assert.AreEqual(SgfDouble.Emphasized, propertyValue.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void ParsingInvalidDoubleThrows()
        {
            var propertyValue = SgfDoubleValue.Parse("3");            
        }

        [TestMethod]
        public void SerializationOfSimpleDoubleWorks()
        {
            var propertyValue = new SgfDoubleValue(SgfDouble.Simple);
            Assert.AreEqual("1", propertyValue.Serialize());
        }

        [TestMethod]
        public void SerializationOfEmphasizedDoubleWorks()
        {
            var propertyValue = new SgfDoubleValue(SgfDouble.Emphasized);
            Assert.AreEqual("2", propertyValue.Serialize());
        }
    }
}
