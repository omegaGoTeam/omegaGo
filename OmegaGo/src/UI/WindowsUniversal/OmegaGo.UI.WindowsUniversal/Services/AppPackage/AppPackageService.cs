using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using OmegaGo.UI.Services.AppPackage;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.WindowsUniversal.Services.AppPackage
{
    /// <summary>
    /// Provides information about the application package
    /// </summary>
    public class AppPackageService : IAppPackageService
    {
        private readonly Localizer _localizer = new Localizer();

        /// <summary>
        /// Gets the app name
        /// </summary>
        public string AppName => Package.Current.DisplayName;

        /// <summary>
        /// Gets the app version
        /// </summary>
        public string Version => $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}";
    }
}
