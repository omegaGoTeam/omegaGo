using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Files
{
    /// <summary>
    /// Works with files in the app package
    /// </summary>
    public interface IAppPackageFileService
    {
        /// <summary>
        /// Reads a content file in app package
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Content of the file</returns>
        Task<string> ReadFileFromRelativePathAsync( string filePath );

        /// <summary>
        /// Gets the file tree within a given folder of the app package content
        /// </summary>
        /// <param name="folder">Folder to enumerate</param>
        /// <param name="includeSubFolders">Should subfolders be included?</param>
        /// <returns>Absolute paths to all files</returns>
        Task<IEnumerable<string>> GetFilePathsAsync(string folder, bool includeSubFolders );
    }
}
