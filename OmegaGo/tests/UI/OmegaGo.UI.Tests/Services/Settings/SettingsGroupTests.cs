using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OmegaGo.UI.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Tests.Services.Settings
{
    [TestClass]
    public class SettingsGroupTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SettingsGroupKeyCannotBeNull()
        {
            var settingsService = new Mock<ISettingsService>();
            TestSettingsGroup group = new TestSettingsGroup(null, settingsService.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SettingsGroupServiceCannotBeNull()
        {
            var settingsService = new TestSettingsGroup("Test", null);
        }

        [TestMethod]
        public void SettingRetrievalFiresSettingsServiceWithTheRightKey()
        {
            var settingsService = new Mock<ISettingsService>();            
            TestSettingsGroup group = new TestSettingsGroup("Prefix", settingsService.Object);
            var localProperty = group.SampleProperty;
            settingsService.Verify(s => s.GetSetting("Prefix_" + nameof(group.SampleProperty), It.IsAny<Func<string>>(), It.IsAny<SettingLocality>()));
        }

        [TestMethod]
        public void SettingStoreFiresSettingsServiceWithTheRightKey()
        {
            var settingsService = new Mock<ISettingsService>();            
            TestSettingsGroup group = new TestSettingsGroup("Prefix", settingsService.Object);
            group.SampleProperty = "test";
            settingsService.Verify(s => s.SetSetting("Prefix_" + nameof(group.SampleProperty), It.IsAny<string>(), It.IsAny<SettingLocality>()));
        }
    }
}