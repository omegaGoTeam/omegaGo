using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core
{
    /// <summary>
    /// Represents a color of stones in a game of two-player Go. This enumeration is used both for identifying players
    /// and the stones placed on the board.
    /// </summary>
    public enum Color : byte
    {
        /// <summary>
        /// An intersection has the color <see cref="None"/> if there is no stone placed upon it.
        /// </summary>
        None,
        Black,
        White
    }
}
