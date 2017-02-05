using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame
{
    public enum GameEndReason
    {
        /// <summary>
        /// The game ended through normal progress of gameplay.
        /// </summary>
        ScoringComplete,
        /// <summary>
        /// A player lost on time because they did not have any time left.
        /// </summary>
        Timeout,
        /// <summary>
        /// A player has resigned.
        /// </summary>
        Resignation,
        /// <summary>
        /// A player has disconnected from the server.
        /// </summary>
        Disconnection,
        /// <summary>
        /// The game was cancelled without a result, perhaps because the server
        /// decided that it will be so or because it was a local game that was aborted
        /// by user without result.
        /// </summary>
        Cancellation
    }
}
