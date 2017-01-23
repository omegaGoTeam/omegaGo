using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.Common
{
    /// <summary>
    /// Random values provider
    /// </summary>
    internal class Randomness
    {
        private static readonly System.Random Randomizer = new System.Random();

        public static int Next(int maximum)
        {
            return Randomizer.Next(maximum);
        }
    }
}
