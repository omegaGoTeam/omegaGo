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
    public class SgfComposePropertyValueTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SgfComposeNullParseThrows()
        {
            SgfComposePropertyValue<string, string>.Parse(null, SgfNumberValue.Parse, SgfNumberValue.Parse);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SgfComposeLeftParserNullParseThrows()
        {
            SgfComposePropertyValue<string, string>.Parse("1:2", null, SgfNumberValue.Parse);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SgfComposeRightParserNullParseThrows()
        {
            SgfComposePropertyValue<string, string>.Parse("1:2", SgfNumberValue.Parse, null);
        }

        [TestMethod]
        public void ValidSgfComposeValueCanBeParsed()
        {
            var compose = SgfComposePropertyValue<string, int>.Parse("hello wae:1", SgfTextValue.Parse, SgfNumberValue.Parse);
            Assert.AreEqual("hello wae", compose.LeftValue);
            Assert.AreEqual(1, compose.RightValue);
        }

        [TestMethod]
        public void SgfComposeValueWithEscapedColonsCanBeParsed()
        {
            var compose = SgfComposePropertyValue<string, string>.Parse("hell\\:o wae\\\\:\\:\\\\\\:", SgfTextValue.Parse, SgfSimpleTextValue.Parse);
            Assert.AreEqual("hell:o wae\\", compose.LeftValue);
            Assert.AreEqual(":\\:", compose.RightValue);
        }

        [TestMethod]
        public void SgfComposeValueIsProperlySerialized()
        {
            var compose = new SgfComposePropertyValue<string,string>(new SgfTextValue("Hello, wo:rld"), new SgfTextValue("::::") );
            Assert.AreEqual("Hello, wo\\:rld:\\:\\:\\:\\:", compose.Serialize());
        }
    }
}
