namespace OmegaGo.UI.WindowsUniversal.Helpers.Device
{
    /// <summary>
    /// Trigger for switching between Windows and Windows Phone
    /// </summary>
    public class DeviceFamilyHelper
    {
        /// <summary>
        /// Gets or sets the device family to trigger on.
        /// </summary>
        /// <value>The device family.</value>
        public static DeviceFamily DeviceFamily
        {
            get
            {
                var deviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                switch (deviceFamily)
                {
                    case "Windows.Mobile":
                        return DeviceFamily.Mobile;
                    case "Windows.Desktop":
                        return DeviceFamily.Desktop;
                    case "Windows.Team":
                        return DeviceFamily.Team;
                    case "Windows.IoT":
                        return DeviceFamily.IoT;
                    case "Windows.Holographic":
                        return DeviceFamily.Holographic;
                    case "Windows.Xbox":
                        return DeviceFamily.Xbox;
                    default:
                        return DeviceFamily.Unknown;
                }
            }
        }
    }
}
