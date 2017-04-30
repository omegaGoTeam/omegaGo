using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.Markup
{
    /// <summary>
    /// Interface for items (markup or stone), which are shown as a shadow items, when the pointer is over the given intersection.
    /// </summary>
    public interface IShadowItem
    {
        /// <summary>
        /// Type of shadow item.
        /// </summary>
        ShadowItemKind ShadowItemKind { get; }
    }
}
