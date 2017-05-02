using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Markup
{
    /// <summary>
    /// Empty shadow item, that is used, when the intersection contains a markup of same type.
    /// </summary>
    public sealed class None: IShadowItem
    {
        public ShadowItemKind ShadowItemKind => ShadowItemKind.None;
    }
}
