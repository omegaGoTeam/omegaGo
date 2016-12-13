using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Tsumego
{
    class Problems
    {
        static Problems()
        {
            var names = (typeof(Problems).GetTypeInfo().Assembly).GetManifestResourceNames();
            foreach(var name in names)
            {
                if (!name.StartsWith("OmegaGo.UI.Services.Tsumego.")) continue;

                Stream stream = (typeof(Problems).GetTypeInfo().Assembly).GetManifestResourceStream(name);
                StreamReader sr = new StreamReader(stream);
                string data = sr.ReadToEnd();
                TsumegoProblem problem = TsumegoProblem.CreateFromSgfText(data);
                AllProblems.Add(problem);
            }
            if (AllProblems.Count == 0)
            {
                throw new Exception(
                    "At least one problem should have been loaded. Are you sure that the problems are located in the OmegaGo.UI.Services.Tsumego subfolder and that they're set as Embedded Resource?");
            }
        }

        public static readonly List<TsumegoProblem> AllProblems = new List<TsumegoProblem>();
    }
}
