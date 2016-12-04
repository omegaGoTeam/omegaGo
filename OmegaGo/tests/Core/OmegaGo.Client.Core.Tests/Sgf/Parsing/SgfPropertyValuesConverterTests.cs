using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            SgfPropertyValuesConverter.GetValue(null, "someValue");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPropertyValueThrows()
        {
            SgfPropertyValuesConverter.GetValue("C", null);
        }

        [TestMethod]
        public void MadeUpPropertyValueIsUnknown()
        {
            var helloWorld = "Hello world";
            var result = SgfPropertyValuesConverter.GetValue("MYMADEUPPROPERTY", helloWorld);
            Assert.IsTrue(result is SgfUnknownPropertyValue);
            Assert.AreEqual(helloWorld, result.Serialize());
        }
    }
}
