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
    public class SgfUnknownPropertyValueTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SgfUnknownPropertyValueDoesNotAcceptNullValue()
        {
            var property = new SgfUnknownPropertyValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SgfUnknownPropertyValueParsingDoesNotAcceptNullValue()
        {
            var result = SgfUnknownPropertyValue.Parse(null);
        }

        [TestMethod]
        public void ValidUnknownValueParseReturnsInstanceOfUnknown()
        {
            var result = SgfUnknownPropertyValue.Parse("someValue");
            Assert.IsTrue(result is SgfUnknownPropertyValue);
        }

        [TestMethod]
        public void UnknownValueIsNotAffectedByParsing()
        {
            var value = "asdfasoifewq34-93-24;te/,;t[s54;366]2==IUW_935 rt t\\q rs  \n\r asfp842-36";
            var property = (SgfUnknownPropertyValue)SgfUnknownPropertyValue.Parse(value);

            Assert.AreEqual(value, property.Value);
        }

        [TestMethod]
        public void UnknownValueIsNotAffectedBySerializing()
        {
            var value = "asdfasoifewq34-93-24;te/,;t[s54;366]2==IUW_935 rt t\\q rs  \n\r asfp842-36";
            var property = new SgfUnknownPropertyValue(value);

            Assert.AreEqual(value, property.Serialize());
        }
    }
}
