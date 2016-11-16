using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents the root of the SGF tree
    /// </summary>
    internal class SgfRoot
    {
        public List<SgfProperty> GlobalCommands { get; } = new List<SgfProperty>();
        public List<SgfGameTree> Games { get; } = new List<SgfGameTree>();
    }
}
