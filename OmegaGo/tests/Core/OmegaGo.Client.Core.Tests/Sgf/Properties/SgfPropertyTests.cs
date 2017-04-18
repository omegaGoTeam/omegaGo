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
    public class SgfPropertyTests
    {
        [TestMethod]
        public void NullPropertyIdentiferIsInvalid()
        {
            Assert.IsFalse(SgfProperty.IsPropertyIdentifierValid(null));
        }

        [TestMethod]
        public void NullPropertyTypeIsInvalid()
        {
            Assert.AreEqual(SgfPropertyType.Invalid, SgfProperty.GetPropertyType(null));
        }

        [TestMethod]
        public void GameInfoPropertyIsRecognized()
        {
            Assert.AreEqual(SgfPropertyType.GameInfo, SgfProperty.GetPropertyType("ON"));
        }

        [TestMethod]
        public void RootPropertyIsRecognized()
        {
            Assert.AreEqual(SgfPropertyType.Root, SgfProperty.GetPropertyType("CA"));
        }

        [TestMethod]
        public void MovePropertyIsRecognized()
        {
            Assert.AreEqual(SgfPropertyType.Move, SgfProperty.GetPropertyType("DO"));
        }

        [TestMethod]
        public void SetupPropertyIsRecognized()
        {
            Assert.AreEqual(SgfPropertyType.Setup, SgfProperty.GetPropertyType("AB"));
        }

        [TestMethod]
        public void NoTypePropertyIsRecognized()
        {
            Assert.AreEqual(SgfPropertyType.NoType, SgfProperty.GetPropertyType("LN"));
        }

        [TestMethod]
        public void DeprecatedPropertyIsRecognized()
        {
            Assert.AreEqual(SgfPropertyType.Deprecated, SgfProperty.GetPropertyType("RG"));
        }

        [TestMethod]
        public void ValidUnknownPropertyWithOneLetterIsRecognized()
        {
            Assert.AreEqual(SgfPropertyType.Unknown, SgfProperty.GetPropertyType("Y"));
        }

        [TestMethod]
        public void ValidUnknownPropertyWithTwoLettersIsRecognized()
        {
            Assert.AreEqual(SgfPropertyType.Unknown, SgfProperty.GetPropertyType("SS"));
        }

        [TestMethod]
        public void ValidUnknownPropertyWithMoreLettersIsRecognized()
        {
            Assert.AreEqual(SgfPropertyType.Unknown, SgfProperty.GetPropertyType("SSSSSS"));
        }

        [TestMethod]
        public void PropertyIdentifierWithNonCaptialLettersIsInvalid()
        {
            Assert.AreEqual(SgfPropertyType.Invalid, SgfProperty.GetPropertyType("ab"));
        }

        [TestMethod]
        public void PropertyIdentifierWithInvalidCharactersIsInvalid()
        {
            Assert.AreEqual(SgfPropertyType.Invalid, SgfProperty.GetPropertyType(":-"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NullPropertyInstanceCantBeCreated()
        {
            new SgfProperty(null, new SgfNumberValue(4));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidPropertyInstanceCantBeCreated()
        {
            new SgfProperty("ff", new[] { new SgfNumberValue(4) });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidPropertyWithNullValuesCantBeCreated()
        {
            new SgfProperty("FF", null);
        }

        [TestMethod]
        public void ValidPropertyWithValidValuesIsCreatedSuccessfully()
        {
            var sgfProperty = SgfProperty.ParseValuesAndCreate("AW", "ab", "ac", "bb", "bc");
            Assert.AreEqual("AW", sgfProperty.Identifier);
            Assert.AreEqual(4, sgfProperty.SimpleValues<SgfPointRectangle>().Count());
        }

        [TestMethod]
        public void ValidPropertyWithValidComposeValuesIsCreatedSuccessfully()
        {
            var sgfProperty = SgfProperty.ParseValuesAndCreate("LB", "ab:Hello world!", "ac:Goodbye!");
            Assert.AreEqual("LB", sgfProperty.Identifier);
            Assert.AreEqual(2, sgfProperty.ComposeValues<SgfPoint, string>().Count());
        }

        [TestMethod]
        public void ValueTypeOfListPropertyIsCorrectlyReturned()
        {
            var sgfProperty = SgfProperty.ParseValuesAndCreate("AW", "ab", "ac", "bb", "bc");
            Assert.AreEqual(SgfValueType.PointRectangle, sgfProperty.ValueType);
        }

        [TestMethod]
        public void ValueTypeOfNonePropertyIsCorrectlyReturned()
        {
            var sgfProperty = new SgfProperty("KO");
            Assert.AreEqual(SgfValueType.None, sgfProperty.ValueType);
        }
    }
}
