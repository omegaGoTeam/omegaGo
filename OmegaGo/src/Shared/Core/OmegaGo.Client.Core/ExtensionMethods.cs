using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    static class ExtensionMethods
    {
        public static int AsInteger(this string text)
        {
           
                return int.Parse(text);
           
        }
        public static float AsFloat(this string text)
        {
            // We can refactor this later.
            return float.Parse(text, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture);
        }
    }
}
