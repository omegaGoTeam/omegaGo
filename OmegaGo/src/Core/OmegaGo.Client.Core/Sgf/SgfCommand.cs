using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf
{
    internal class SgfProperty
    {
        public string Identifier { get; set; }
        public List<string> Values { get; set; } = new List<string>();
    }
}
