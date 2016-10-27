using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.Tests.Services.Localization
{
    [TestClass]
    public class LocalizationServiceTests
    {
        [TestMethod]
        public void IndexerReturnsDefaultTranslation()
        {
            LocalizationService localizer = new LocalizationService( LocalizationServiceTestResources.ResourceManager );
            string result = localizer[ "Test" ];
            Assert.AreEqual( "Default", result );
        }

        [TestMethod]
        public void GetStringReturnsDefaultTranslation()
        {
            LocalizationService localizer = new LocalizationService( LocalizationServiceTestResources.ResourceManager );
            string result = localizer.GetString( "Test" );
            Assert.AreEqual( "Default", result );
        }

        [TestMethod]
        public void GetStringReturnsDifferentTranslationWhenCultureProvided()
        {
            LocalizationService localizer = new LocalizationService( LocalizationServiceTestResources.ResourceManager );
            string result = localizer.GetString( "Test", CultureInfo.GetCultureInfo( "cs" ) );
            Assert.AreEqual( "Localized", result );
        }

        [TestMethod]
        public void LocalizationReturnsKeyIfNotFound()
        {
            LocalizationService localizer = new LocalizationService( LocalizationServiceTestResources.ResourceManager );
            string nonExistingKey = "SomethingNonExistent";
            string result = localizer[ nonExistingKey ];
            Assert.AreEqual( nonExistingKey, result );
        }

        [TestMethod]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void LocalizationServiceThrowsForNullConstructorArgument()
        {
            LocalizationService localizer = new LocalizationService( null );
        }

        [TestMethod]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void LocalizationServiceThrowsForNullKey()
        {
            LocalizationService localizer = new LocalizationService( LocalizationServiceTestResources.ResourceManager );
            localizer.GetString( null );
        }

        [TestMethod]
        [ExpectedException( typeof( ArgumentNullException ) )]
        public void LocalizationServiceThrowsForNullCulture()
        {
            LocalizationService localizer = new LocalizationService( LocalizationServiceTestResources.ResourceManager );
            localizer.GetString( "Test", null );
        }

        [TestMethod]
        public void LocalizeCallerLocalizesProperly()
        {
            TestLocalizationService localizer = new TestLocalizationService();
            Assert.AreEqual( "Default", localizer.Test );
        }
    }
}
