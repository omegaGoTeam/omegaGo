using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Tools
{
    interface IToolServices
    {
        Position PointerOverPosition { get; set; }
        GameTreeNode Node { get; set; }
    }
}
