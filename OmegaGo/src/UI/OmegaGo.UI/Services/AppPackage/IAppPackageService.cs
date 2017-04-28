using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.AppPackage
{
    /// <summary>
    /// Provides access to information about the application package
    /// </summary>
    public interface IAppPackageService
    {
        /// <summary>
        /// App name
        /// </summary>
        string AppName { get; }

        /// <summary>
        /// App version
        /// </summary>
        string Version { get; }
    }
}
