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
    public class SgfSimpleTextValueTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullTextCannotBeCreated()
        {
            var value = new SgfSimpleTextValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullValueCannotBeParsed()
        {
            SgfSimpleTextValue.Parse(null);
        }

        [TestMethod]
        public void SimpleStringCanBeParsed()
        {
            var propertyValue = SgfSimpleTextValue.Parse("Hello, world.");
            Assert.AreEqual("Hello, world.", propertyValue.Value);
        }

        [TestMethod]
        public void SimpleStringWithWhitespaceCanBeParsed()
        {
            var propertyValue = SgfSimpleTextValue.Parse("Hello,\t world.");
            Assert.AreEqual("Hello,  world.", propertyValue.Value);
        }

        [TestMethod]
        public void StringWithNewlinesCanBeParsed()
        {
            var propertyValue = SgfSimpleTextValue.Parse("Hello,\r \n \r\n \n\rworld.");
            Assert.AreEqual(
                $"Hello,       world.",
                propertyValue.Value);
        }

        [TestMethod]
        public void StringWithEscapedNewlinesCanBeParsed()
        {
            var propertyValue = SgfSimpleTextValue.Parse("Hello,\\\r world.");
            Assert.AreEqual(
                $"Hello, world.",
                propertyValue.Value);
        }

        [TestMethod]
        public void MoreComplexTextIsProperlyParsed()
        {
            var actualValue =
                @"Meijin NR: yeah, k4 is won\
derful
sweat NR: thank you! :\)
dada NR: yup. I like this move too. It's a move only to be expected from a pro. I really like it :)
jansteen 4d: Can anyone\
 explain [me\] k4?";
            var expected =
                @"Meijin NR: yeah, k4 is wonderful sweat NR: thank you! :) dada NR: yup. I like this move too. It's a move only to be expected from a pro. I really like it :) jansteen 4d: Can anyone explain [me] k4?";
            var propertyValue = SgfSimpleTextValue.Parse(actualValue);
            var actual = propertyValue.Value;
            TestUtilities.AssertStringEqualityByCharacters(expected, actual);
        }
    }
}
