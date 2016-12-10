using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf.Properties;
using OmegaGo.Core.Sgf.Properties.Values;
using OmegaGo.Core.Sgf.Properties.Values.ValueTypes;

namespace OmegaGo.Core.Tests.Sgf.Properties
{
    [TestClass]
    public class SgfKnownPropertiesTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryingToGetNullKnownPropertyThrows()
        {
            SgfKnownProperties.Get(null);
        }

        [TestMethod]
        public void SimpleBlackMovePropertyIsKnown()
        {
            var blackMoveProperty = SgfKnownProperties.Get("B");
            Assert.AreEqual("B", blackMoveProperty.Identifier);
            Assert.AreEqual(SgfPointValue.Parse, blackMoveProperty.Parser);
            Assert.AreEqual(SgfValueMultiplicity.Single, blackMoveProperty.ValueMultiplicity);
            Assert.AreEqual(SgfPropertyType.Move, blackMoveProperty.Type);
        }

        [TestMethod]
        public void ContainsCanCheckForKnownProperties()
        {
            var containsResult = SgfKnownProperties.Contains("TW");
            Assert.IsTrue(containsResult);
        }
    }
}
