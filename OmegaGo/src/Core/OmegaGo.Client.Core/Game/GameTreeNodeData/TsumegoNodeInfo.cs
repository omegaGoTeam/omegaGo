using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Game.GameTreeNodeData
{
    /// <summary>
    /// Tsumego-related game tree node info
    /// </summary>
    public class TsumegoNodeInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether reaching this node means the player has successfully
        /// solved a tsumego problem.
        /// </summary>
        public bool Correct { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether reaching this node means that further exploration of 
        /// this branch will not yield a correct solution to a tsumego problem, and the problem should count
        /// as failed.
        /// </summary>
        public bool Wrong { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this node is part of the tsumego definition. Other nodes
        /// would be created by the player, are termed 'unexpected' and are considered wrong.
        /// </summary>
        public bool Expected { get; set; }

        /// <summary>
        /// Gets the list of positions that are known as possible continuation from this node to the author
        /// of the tsumego problem that contains this node.
        /// </summary>
        public List<Position> MarkedPositions { get; } = new List<Position>();

        public override string ToString()
        {
            return (Correct ? "[correct]" : "") + (Wrong ? "[wrong]" : "") + (Expected ? "[Expected]" : "") +
                   (MarkedPositions.Any() ? "[" + MarkedPositions.Count + " marked]" : "");
        }
    }
}
