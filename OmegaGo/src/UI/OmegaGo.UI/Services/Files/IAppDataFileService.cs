using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Files
{
    /// <summary>
    /// Provides platform independent file read and write support.
    /// </summary>
    public interface IAppDataFileService
    {
        /// <summary>
        /// Reads the content of the specified file in app data folder
        /// </summary>
        Task<string> ReadFileAsync(string subfolder, string filename);

        /// <summary>
        /// Writes text to the target file in app data folder
        /// </summary>
        Task WriteFileAsync(string subfolder, string filename, string fileContent);

        /// <summary>
        /// If the specified path does not exist within the app data folder, it is created.
        /// </summary>
        Task EnsureFolderExistsAsync(string folderPath);
        
        /// <summary>
        /// Gets the list of all files within the given folder that's inside the app's local folder. The filenames returned are given relative
        /// to their immediate parent folder, i.e. they contain no backslashes.
        /// </summary>
        /// <param name="folderPath">The folder that should have its files enumerated.</param>
        /// <returns>List of files</returns>
        Task<IEnumerable<string>> EnumerateFilesInFolderAsync(string folderPath);

        /// <summary>
        /// Launches File Explorer opened to the specified folder.
        /// </summary>
        Task LaunchFolderAsync(string folderPath);

        /// <summary>
        /// Attempts to delete a file.
        /// </summary
        Task DeleteFileAsync(string subfolder, string filename);

        /// <summary>
        /// Checks if a file exists
        /// </summary>
        /// <param name="subfolder">Subfolder</param>
        /// <param name="filename">File name</param>
        /// <returns>Does the file exist?</returns>
        Task<bool> FileExistsAsync(string subfolder, string filename);
    }
}
