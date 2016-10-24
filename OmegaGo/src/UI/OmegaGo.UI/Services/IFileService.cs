using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services
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
    }
}
