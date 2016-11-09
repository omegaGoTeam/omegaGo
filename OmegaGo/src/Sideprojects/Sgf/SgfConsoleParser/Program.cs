using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf;

namespace SgfConsoleParser
{
    class Program
    {
        static void Main(string[] args)
        {
            SgfGameTreeSerializer serializer =
                new SgfGameTreeSerializer();
            serializer.Deserialize(File.ReadAllText("C:\\Users\\Martin\\Downloads\\ff4_ex.sgf"));
        }
    }
}
