using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// Represents a tree branch in SGF file
    /// </summary>
    internal class SgfTreeNode
    {
        public List<SgfNode> Nodes { get; set; } = new List<SgfNode>();

        public List<SgfTreeNode> Children { get; set; } = new List<SgfTreeNode>();
    }
}
