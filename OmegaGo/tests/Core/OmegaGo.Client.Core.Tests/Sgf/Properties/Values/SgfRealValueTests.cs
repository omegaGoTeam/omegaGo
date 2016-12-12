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
    public class SgfRealValueTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullRealValueCantBeParsed()
        {
            SgfRealValue.Parse(null);
        }

        [TestMethod]
        public void SimpleIntegerValueCanBeParsed()
        {
            var value = SgfRealValue.Parse("12");
            Assert.AreEqual(12, value.Value);
        }

        [TestMethod]
        public void SimpleRealValueCanBeParsed()
        {
            var value = SgfRealValue.Parse("12.13");
            Assert.AreEqual(12.13m, value.Value);
        }

        [TestMethod]
        public void PositiveIntegerValueCanBeParsed()
        {
            var value = SgfRealValue.Parse("+12");
            Assert.AreEqual(12, value.Value);
        }

        [TestMethod]
        public void NegativeIntegerValueCanBeParsed()
        {
            var value = SgfRealValue.Parse("-12");
            Assert.AreEqual(-12, value.Value);
        }

        [TestMethod]
        public void NegativeRealValueCanBeParsed()
        {
            var value = SgfRealValue.Parse("-12.13");
            Assert.AreEqual(-12.13m, value.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void EmptyValueCantBeParsed()
        {
            var value = SgfRealValue.Parse("");
        }

        [TestMethod]
        public void IntegerValueIsProperlySerialized()
        {
            var value = new SgfRealValue(12);
            Assert.AreEqual("12", value.Serialize());
        }

        [TestMethod]
        public void NegativeIntegerValueIsProperlySerialized()
        {
            var value = new SgfRealValue(-12);
            Assert.AreEqual("-12", value.Serialize());
        }

        [TestMethod]
        public void RealValueIsProperlySerialized()
        {
            var value = new SgfRealValue(12.63m);
            Assert.AreEqual("12.63", value.Serialize());
        }

        [TestMethod]
        public void NegativeRealValueIsProperlySerialized()
        {
            var value = new SgfRealValue(-12.63m);
            Assert.AreEqual("-12.63", value.Serialize());
        }
    }
}
