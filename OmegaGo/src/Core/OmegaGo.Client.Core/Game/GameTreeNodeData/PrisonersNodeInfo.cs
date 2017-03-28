using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.GameTreeNodeData
{
    /// <summary>
    /// Contains the number of prisoners of each player at the point the game was in this node. This is unused
    /// in Analyze Mode, in tsumego or in tutorial.
    /// </summary>
    public class PrisonersNodeInfo
    {
        /// <summary>
        /// Gets or sets the number of stones captured by black from the beginning of the game up to now.
        /// </summary>
        public int BlackPrisoners { get; set; }
        /// <summary>
        /// Gets or sets the number of stones captured by white from the beginning of the game up to now.
        /// </summary>
        public int WhitePrisoners { get; set; }
    }
}
