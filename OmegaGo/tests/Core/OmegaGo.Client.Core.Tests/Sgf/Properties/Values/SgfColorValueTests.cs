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
    public class SgfColorValueTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullSgfColorParsingThrows()
        {
            SgfColorValue.Parse(null);
        }

        [TestMethod]
        public void BlackColorIsProperlyParsed()
        {
            var black = SgfColorValue.Parse("B");
            Assert.AreEqual(SgfColor.Black, black.Value);
        }

        [TestMethod]
        public void WhiteColorIsProperlyParsed()
        {
            var white = SgfColorValue.Parse("W");
            Assert.AreEqual(SgfColor.White, white.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void OutOfRangeColorThrowsInConstructor()
        {
            var sgfColor = new SgfColorValue((SgfColor)123);            
        }

        [TestMethod]
        public void SerializationOfBlackColorWorks()
        {
            var sgfColor = new SgfColorValue(SgfColor.Black);
            Assert.AreEqual("B", sgfColor.Serialize());
        }

        [TestMethod]
        public void SerializationOfWhiteColorWorks()
        {
            var sgfColor = new SgfColorValue(SgfColor.White);
            Assert.AreEqual("W", sgfColor.Serialize());
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void ParsingInvalidLowercaseValueThrows()
        {
            SgfColorValue.Parse("b");
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void ParsingInvalidValueThrows()
        {
            SgfColorValue.Parse("A");
        }
    }
}
