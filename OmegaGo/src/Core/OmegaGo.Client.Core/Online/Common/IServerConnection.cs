using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Remote;

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
        /// Contains events that can be triggered by all online servers. <see cref="RemoteGameController"/> and viewmodels may use these without
        /// requiring specialization.
        /// </summary>
        ICommonEvents Events { get; }

        /// <summary>
        /// Gets information whether this is IGS or KGS.
        /// </summary>
        // ReSharper disable once UnusedMember.Global : yes, currently unusued, but it makes sense 
        ServerId Name { get; }
    }
}
