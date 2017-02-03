using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Common
{
    /// <summary>
    /// A server connection class is how we communicate with an online server. This interface contains
    /// methods and events common to all online servers that we support.
    /// </summary>
    public interface IServerConnection
    {
        /// <summary>
        /// Commands are used when our client wants to send information to the server (such as making a move).
        /// </summary>
        ICommonCommands Commands { get; }
        /// <summary>
        /// Events are used when the server wants to send information to the client
        /// (such as giving us additional time).
        /// </summary>
        ICommonEvents Events { get; }
    }
}
