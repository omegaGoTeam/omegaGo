using OmegaGo.Core.Game.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Tools
{
    interface IMarkupTool : ITool
    {
        MarkupKind Markup { get; }
    }
}
