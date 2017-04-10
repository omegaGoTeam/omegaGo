using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Tsumego
{
    /// <summary>
    /// Loads and holds Tsumego problem statements.
    /// </summary>
    static class Problems
    {
        /// <summary>
        /// Initializes the <see cref="Problems"/> static class by loading all embedded tsumego SGF files.
        /// </summary>
        static Problems()
        {
            var names = (typeof(Problems).GetTypeInfo().Assembly).GetManifestResourceNames();
            foreach (var name in names)
            {
                if (!name.StartsWith("OmegaGo.UI.Services.Tsumego.")) continue;

                Stream stream = (typeof(Problems).GetTypeInfo().Assembly).GetManifestResourceStream(name);
                StreamReader sr = new StreamReader(stream);
                string data = sr.ReadToEnd();
                TsumegoProblem problem = TsumegoProblem.CreateFromSgfText(data);
                Problems.allProblems.Add(problem);
            }
            if (Problems.allProblems.Count == 0)
            {
                throw new Exception(
                    "At least one problem should have been loaded. Are you sure that the problems are located in the OmegaGo.UI.Services.Tsumego subfolder and that they're set as Embedded Resource?");
            }
        }

        private static readonly List<TsumegoProblem> allProblems = new List<TsumegoProblem>();
        /// <summary>
        /// Gets all embedded tsumego problem statements.
        /// </summary>
        public static IList<TsumegoProblem> AllProblems => Problems.allProblems;
    }
}
