using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Files
{
    /// <summary>
    /// Allows file picking
    /// </summary>
    public interface IFilePickerService
    {
        /// <summary>
        /// Allows the user to pick a file and returns its contents
        /// </summary>
        /// <param name="extensions">Allowed file extensions</param>
        /// <returns>File contents or null</returns>
        Task<FileContentInfo> PickAndReadFileAsync( params string[] extensions );

        /// <summary>
        /// Suggested file name
        /// </summary>
        /// <param name="suggestedFileName">Suggested file name</param>
        /// <param name="contents">Text to write in the file</param>        
        Task PickAndWriteFileAsync(string suggestedFileName, string contents);
    }
}
