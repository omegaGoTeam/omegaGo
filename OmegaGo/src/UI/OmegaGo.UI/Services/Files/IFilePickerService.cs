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
        Task<string> PickAndReadFileAsync( params string[] extensions );
    }
}
