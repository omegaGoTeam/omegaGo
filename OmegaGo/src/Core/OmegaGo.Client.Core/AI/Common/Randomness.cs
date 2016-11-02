using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Common
{
    class Randomness
    {
        private static System.Random rgen = new System.Random();
        public static int Next(int maximum)
        {
            return rgen.Next(maximum);
        }
    }
}
