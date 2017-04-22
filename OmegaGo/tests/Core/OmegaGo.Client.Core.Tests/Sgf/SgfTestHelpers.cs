using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf;
using OmegaGo.Core.Sgf.Parsing;

namespace OmegaGo.Core.Tests.Sgf
{
    public static class SgfTestHelpers
    {
        public static SgfCollection ParseFile(SgfParser parser, string sampleSgfSubPath)
        {
            return parser.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
                "Sgf/Parsing/SampleSgfs/", sampleSgfSubPath)));
        }

        public static string[] GetSgfFiles(string sgfFolder)
        {
            return Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(),
                "Sgf/Parsing/SampleSgfs/", sgfFolder));
        }
    }
}
