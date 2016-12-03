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
        [TestMethod]
        public void IntSettingIsProperlyStoredLocally()
        {
            int value = randomizer.Next();
            var settingKey = "IntValueLocal";
            SettingsService settingsService = new SettingsService();
            settingsService.SetSetting(settingKey, value);
            var retrieved = settingsService.GetSetting(settingKey, () => (value * 2));
            Assert.AreEqual(value, retrieved);
        }
    }
}
