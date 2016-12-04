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
            var property = new SgfUnknownValue(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SgfUnknownPropertyValueParsingDoesNotAcceptNullValue()
        {
            var result = SgfUnknownValue.Parse(null);
        }

        [TestMethod]
        public void ValidUnknownValueParseReturnsInstanceOfUnknown()
        {
            var result = SgfUnknownValue.Parse("someValue");
            Assert.IsTrue(result is SgfUnknownValue);
        }

        [TestMethod]
        public void UnknownValueIsNotAffectedByParsing()
        {
            var value = "asdfasoifewq34-93-24;te/,;t[s54;366]2==IUW_935 rt t\\q rs  \n\r asfp842-36";
            var property = (SgfUnknownValue)SgfUnknownValue.Parse(value);

            Assert.AreEqual(value, property.Value);
        }

        [TestMethod]
        public void UnknownValueIsNotAffectedBySerializing()
        {
            var value = "asdfasoifewq34-93-24;te/,;t[s54;366]2==IUW_935 rt t\\q rs  \n\r asfp842-36";
            var property = new SgfUnknownValue(value);

            Assert.AreEqual(value, property.Serialize());
        }

        [TestMethod]
        public void UnkownValueIdentifiesItselfCorrectly()
        {
            var value = new SgfUnknownValue("asdf");
            Assert.AreEqual(SgfValueType.Unknown, value.ValueType);
        }
    }
}
