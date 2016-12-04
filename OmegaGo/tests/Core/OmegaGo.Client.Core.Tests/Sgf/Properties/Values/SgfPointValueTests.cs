using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf.Properties.Values;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Tests.Sgf.Properties.Values
{
    [TestClass]
    public class SgfPointValueTests
    {
        //because parsing is performed by the SgfPoint struct, related tests are in its test class

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SgfPointParsingThrowsForNull()
        {
            SgfPointValue.Parse(null);
        }

        [TestMethod]
        public void SgfPointSerializationWorks()
        {
            var propertyValue = new SgfPointValue(new SgfPoint(3,5));
            Assert.AreEqual( "df", propertyValue.Serialize());
        }

        [TestMethod]
        public void SgfPointValueParseReturnsItsInstance()
        {
            var propertyValue = SgfPointValue.Parse("aA");
            Assert.IsTrue(propertyValue is SgfPointValue);
        }
    }
}
