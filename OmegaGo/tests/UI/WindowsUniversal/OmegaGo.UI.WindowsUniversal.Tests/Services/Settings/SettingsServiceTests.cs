using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.WindowsUniversal.Services.Settings;

namespace OmegaGo.UI.WindowsUniversal.Tests.Services.Settings
{
    [TestClass]
    public class SettingsServiceTests
    {
        private static Random randomizer = new Random();
        private const double DoubleDelta = 0.000001;

        [TestMethod]
        public void IntSettingIsProperlyStoredLocally()
        {
            int value = randomizer.Next();
            var settingKey = "IntValueLocal";
            SettingsService settingsService = new SettingsService();
            settingsService.SetSetting(settingKey, value);
            var retrieved = settingsService.GetSetting(settingKey, () => -1);
            Assert.AreEqual(value, retrieved);
        }

        [TestMethod]
        public void DoubleSettingIsProperlyStoredLocally()
        {
            double value = randomizer.NextDouble();
            var settingKey = "DoubleValueLocal";
            SettingsService settingsService = new SettingsService();
            settingsService.SetSetting(settingKey, value);
            var retrieved = settingsService.GetSetting(settingKey, () => (2.0));
            Assert.AreEqual(value, retrieved, DoubleDelta);
        }
    }
}
