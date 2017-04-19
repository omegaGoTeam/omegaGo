using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Time
{
    /// <summary>
    /// Name of the time control system used.
    /// </summary>
    public enum TimeControlStyle
    {
        /// <summary>
        /// Players have infinite time.
        /// </summary>
        None,
        /// <summary>
        /// When main time elapses, the player loses.
        /// </summary>
        Absolute,
        /// <summary>
        /// After main time elapses, the player takes a series of periods. Each period takes a number of minutes and the player must make a specified number of moves during that period. When they do, another period begins. If they fail, they lose.
        /// </summary>
        Canadian,
        /// <summary>
        /// This is the Japanase byo-yomi system, too complex to be explained in a comment. See http://senseis.xmp.net/?ByoYomi
        /// </summary>
        Japanese
    }
}
