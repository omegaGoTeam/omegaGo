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

        [TestMethod]
        public void StringSettingIsProperlyStoredLocally()
        {
            var value = "someValue";
            var settingKey = "StringValueLocal";
            SettingsService settingsService = new SettingsService();
            settingsService.SetSetting(settingKey, value);
            var retrieved = settingsService.GetSetting<string>(settingKey, () => (null));
            Assert.AreEqual(value, retrieved);
        }

        [TestMethod]
        public void IntSettingIsProperlyStoredRoamed()
        {
            int value = randomizer.Next();
            var settingKey = "IntValueLocal";
            SettingsService settingsService = new SettingsService();
            settingsService.SetSetting(settingKey, value);
            var retrieved = settingsService.GetSetting(settingKey, () => -1);
            Assert.AreEqual(value, retrieved);
        }

        [TestMethod]
        public void DoubleSettingIsProperlyStoredRoamed()
        {
            double value = randomizer.NextDouble();
            var settingKey = "DoubleValueLocal";
            SettingsService settingsService = new SettingsService();
            settingsService.SetSetting(settingKey, value);
            var retrieved = settingsService.GetSetting(settingKey, () => (2.0));
            Assert.AreEqual(value, retrieved, DoubleDelta);
        }

        [TestMethod]
        public void StringSettingIsProperlyStoredRoamed()
        {
            var value = "someValue";
            var settingKey = "StringValueLocal";
            SettingsService settingsService = new SettingsService();
            settingsService.SetSetting(settingKey, value);
            var retrieved = settingsService.GetSetting<string>(settingKey, () => (null));
            Assert.AreEqual(value, retrieved);
        }

        [TestMethod]
        public void ComplexSettingIsProperlyStored()
        {
            var value = new ComplexSettingParent()
            {
                SomeSimpleProperty = "first",
                SecondSimpleProperty = "second",
                SomeNestedProperty = new ComplexSettingChild()
                {
                    SomeNestedProperty = "nestedFirst",
                    SecondNestedProperty = "nestedSecond"
                }
            };
            var settingKey = "complex";
            SettingsService settingsService = new SettingsService();
            settingsService.SetComplexSetting(settingKey, value);
            var retrieved = settingsService.GetComplexSetting<ComplexSettingParent>(settingKey, () => (null));
            Assert.AreEqual(value.SomeSimpleProperty, retrieved.SomeSimpleProperty);
            Assert.AreEqual(value.SecondSimpleProperty, retrieved.SecondSimpleProperty);
            Assert.AreEqual(value.SomeNestedProperty.SomeNestedProperty, retrieved.SomeNestedProperty.SomeNestedProperty);
            Assert.AreEqual(value.SomeNestedProperty.SecondNestedProperty, retrieved.SomeNestedProperty.SecondNestedProperty);
        }
    }


    public class ComplexSettingParent
    {
        public string SomeSimpleProperty { get; set; }
        public string SecondSimpleProperty { get; set; }
        public ComplexSettingChild SomeNestedProperty { get; set; }
    }

    public class ComplexSettingChild
    {
        public string SomeNestedProperty { get; set; }
        public string SecondNestedProperty { get; set; }
    }
}
