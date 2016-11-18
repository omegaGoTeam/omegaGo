using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.Core.Sgf.Properties;

namespace OmegaGo.Core.Tests.Sgf.Properties
{
    [TestClass]
    public class SgfPropertyTests
    {
        [TestMethod]
        public void NullPropertyIdentiferIsInvalid()
        {
            Assert.IsFalse( SgfProperty.IsPropertyIdentifierValid( null ) );
        }

        [TestMethod]
        public void NullPropertyTypeIsInvalid()
        {
            Assert.AreEqual( SgfPropertyType.Invalid, SgfProperty.GetPropertyType( null ) );
        }

        [TestMethod]
        public void GameInfoPropertyIsRecognized()
        {
            Assert.AreEqual( SgfPropertyType.GameInfo, SgfProperty.GetPropertyType( "ON" ) );
        }

        [TestMethod]
        public void RootPropertyIsRecognized()
        {
            Assert.AreEqual( SgfPropertyType.Root, SgfProperty.GetPropertyType( "CA" ) );
        }

        [TestMethod]
        public void MovePropertyIsRecognized()
        {
            Assert.AreEqual( SgfPropertyType.Move, SgfProperty.GetPropertyType( "DO" ) );
        }

        [TestMethod]
        public void SetupPropertyIsRecognized()
        {
            Assert.AreEqual( SgfPropertyType.Setup, SgfProperty.GetPropertyType( "AB" ) );
        }

        [TestMethod]
        public void NoTypePropertyIsRecognized()
        {
            Assert.AreEqual( SgfPropertyType.NoType, SgfProperty.GetPropertyType( "LN" ) );
        }

        [TestMethod]
        public void DeprecatedPropertyIsRecognized()
        {
            Assert.AreEqual( SgfPropertyType.Deprecated, SgfProperty.GetPropertyType( "RG" ) );
        }

        [TestMethod]
        public void ValidUnknownPropertyWithOneLetterIsRecognized()
        {
            Assert.AreEqual( SgfPropertyType.Deprecated, SgfProperty.GetPropertyType( "Y" ) );
        }

        [TestMethod]
        public void ValidUnknownPropertyWithTwoLettersIsRecognized()
        {
            Assert.AreEqual( SgfPropertyType.Deprecated, SgfProperty.GetPropertyType( "SS" ) );
        }

        [TestMethod]
        public void ValidUnknownPropertyWithMoreLettersIsRecognized()
        {
            Assert.AreEqual( SgfPropertyType.Deprecated, SgfProperty.GetPropertyType( "SSSSSS" ) );
        }

        [TestMethod]
        public void PropertyIdentifierWithNonCaptialLettersIsInvalid()
        {
            Assert.AreEqual( SgfPropertyType.Deprecated, SgfProperty.GetPropertyType( "ab" ) );
        }

        [TestMethod]
        public void PropertyIdentifierWithInvalidCharactersIsInvalid()
        {
            Assert.AreEqual( SgfPropertyType.Deprecated, SgfProperty.GetPropertyType( ":-" ) );
        }
    }
}
