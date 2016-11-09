using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents a node in SGF
    /// </summary>
    internal class SgfNode
    {
        public List<SgfCommand> Commands { get; set; } = new List<SgfCommand>();
    }
}
