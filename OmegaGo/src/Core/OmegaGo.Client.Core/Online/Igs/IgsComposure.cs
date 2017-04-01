using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// IGS composure
    /// </summary>
    public enum IgsComposure
    {
        /// <summary>
        /// Connection was not yet established, or was broken, or we logged out.
        /// </summary>
        Disconnected,
        /// <summary>
        /// We are attempting to login as a guest.
        /// </summary>
        InitialHandshake,
        /// <summary>
        /// We are now either logged in as guest or as a user, but we are at prompt.
        /// </summary>
        Ok,
        /// <summary>
        /// Bad password, unexpected problem during handshake or similar happened. We should disconnect as soon as possible.
        /// </summary>
        Confused,
        /// <summary>
        /// We are attempting to login as a registered user.
        /// </summary>
        LoggingIn
    }
}
