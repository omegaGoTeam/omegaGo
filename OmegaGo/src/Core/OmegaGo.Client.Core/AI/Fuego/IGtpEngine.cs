using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.AI.FuegoSpace
{
    /// <summary>
    /// A Go Text Protocol engine is used by an AI to do the main work. In OmegaGo, only
    /// the Fuego AI uses a GTP engine.
    /// </summary>
    public interface IGtpEngine
    {
        /// <summary>
        /// Sends a GTP command to the engine and blocks until the engine returns a result. The result is returned as a string, excluding the starting "= " or "? ".
        /// </summary>
        /// <param name="command">The GTP command to send.</param>
        /// <returns>Command response</returns>
        GtpResponse SendCommand(string command);
    }
}
