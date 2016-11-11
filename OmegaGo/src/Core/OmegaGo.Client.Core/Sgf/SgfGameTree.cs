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
    internal class SgfGameTree
    {
        public List<SgfNode> Nodes { get; } = new List<SgfNode>();

        public List<SgfGameTree> Children { get; } = new List<SgfGameTree>();
    }
}
