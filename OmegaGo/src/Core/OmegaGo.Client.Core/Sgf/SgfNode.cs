using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Sgf.Properties;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents a node in SGF
    /// </summary>
    internal class SgfNode
    {
        public List<SgfProperty> Commands { get; set; } = new List<SgfProperty>();
    }
}
