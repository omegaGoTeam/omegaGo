using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Markup
{
    public sealed class None: IShadowItem
    {
        public ShadowItemKind ShadowItemKind => ShadowItemKind.None;
    }
}
