using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.ViewModels.Tutorial;

namespace OmegaGo.UI.Services
{
    /// <summary>
    /// Utility class that permits easy reading of embedded files.
    /// </summary>
    internal static class EmbeddedResourceReading
    {
        /// <summary>
        /// Gets the contents of a file specified as "OmegaGo.UI.[folders].[filename].[extension]" as a string.
        /// </summary>
        /// <param name="filename">The filename to read, specified in the form "OmegaGo.UI.[folders].[filename].[extension]".</param>
        public static string ReadAllText(string filename)
        {
            var names = (typeof(EmbeddedResourceReading).GetTypeInfo().Assembly).GetManifestResourceNames();
            Stream stream = (typeof(EmbeddedResourceReading).GetTypeInfo().Assembly).GetManifestResourceStream(filename);
            StreamReader sr = new StreamReader(stream);
            string data = sr.ReadToEnd();
            return data;
        }
    }
}
