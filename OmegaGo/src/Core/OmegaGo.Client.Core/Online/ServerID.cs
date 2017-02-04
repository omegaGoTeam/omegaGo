using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online
{
    /// <summary>
    /// This uniquely identifiers an online server. Sometimes comparing with ID's might be preferable
    /// to testing for types.
    /// </summary>
    public enum ServerId
    {
        /// <summary>
        /// The Pandanet Internet Go Server.
        /// </summary>
        Igs,
        /// <summary>
        /// The KGS Go Server.
        /// </summary>
        Kgs
    }
}
