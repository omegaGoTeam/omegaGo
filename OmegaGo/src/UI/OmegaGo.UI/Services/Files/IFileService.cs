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
    public interface IFileService
    {
        /// <summary>
        /// Reads the content of the specified file
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <returns>text content of the file</returns>
        string ReadFile(string filePath);

        /// <summary>
        /// Writes text to the target file
        /// </summary>
        /// <param name="filePath">path to the file</param>
        /// <param name="fileContent">text content</param>
        void WriteFile(string filePath, string fileContent);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string ReadSettingsFile();


        /// <summary>
        /// If the specified path does not exist within the app's local folder, it is created.
        /// </summary>
        void EnsureFolderExists(string folderPath);


        /// <summary>
        /// Gets the list of all files within the given folder that's inside the app's local folder. The filenames returned are given relative
        /// to their immediate parent folder, i.e. they contain no backslashes.
        /// </summary>
        /// <param name="folderPath">The folder that should have its files enumerated.</param>
        /// <returns></returns>
        IEnumerable<string> EnumerateFilesInFolder(string folderPath);
        /// <summary>
        /// Launches File Explorer opened to the specified folder.
        /// </summary>
        Task LaunchFolderAsync(string folderPath);

        /// <summary>
        /// Attempts to delete a file (given as filename relative to the app's local folder).
        /// </summary
        void DeleteFile(string filename);
    }
}
