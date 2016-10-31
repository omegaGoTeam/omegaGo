using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    static class CoreDebug
    {
        static string formingstring = "";
        public static void Write(string text)
        {
            formingstring += text;
        }
        public static void WriteLine()
        {
            Debug.WriteLine(formingstring);
            formingstring = "";
        }
    }
}
