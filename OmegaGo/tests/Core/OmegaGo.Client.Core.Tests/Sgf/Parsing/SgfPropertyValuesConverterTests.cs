using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;
using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Tests.Sgf.Parsing
{
    [TestClass]
    public class SgfPropertyValuesConverterTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPropertyIdentifierThrows()
        {
            SgfPropertyValuesConverter.GetValues(null, "someValue");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPropertyValueThrows()
        {
            SgfPropertyValuesConverter.GetValues("C", null);
        }

        [TestMethod]
        public void MadeUpPropertyValueIsUnknown()
        {
            var helloWorld = "Hello world";
            var result = SgfPropertyValuesConverter.GetValues("MYMADEUPPROPERTY", helloWorld);
            var values = result as ISgfPropertyValue[] ?? result.ToArray();
            Assert.IsTrue(values.First() is SgfUnknownValue);
            Assert.AreEqual(1, values.Count());
            Assert.AreEqual(helloWorld, values.First().Serialize());
        }

        [TestMethod]
        public void KnownListPropertyValuesAreProperlyParsed()
        {
            var result =
                SgfPropertyValuesConverter.GetValues("AB", "aa", "bb", "cc").OfType<SgfPointRectangleValue>().ToList();
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("aa", result[0].Serialize());
            Assert.AreEqual("bb", result[1].Serialize());
            Assert.AreEqual("cc", result[2].Serialize());
        }

        [TestMethod]
        [ExpectedException(typeof(SgfParseException))]
        public void KnownPropertyWithWrongValueTypesParsingThrows()
        {
            SgfPropertyValuesConverter.GetValues("AB", "11", "22", "33").OfType<SgfPointRectangleValue>().ToList();
        }
    }
}
