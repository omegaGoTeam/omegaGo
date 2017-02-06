using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Igs
{
    partial class IgsConnection
    {
        /// <summary>
        /// IGS composure
        /// </summary>
        enum IgsComposure
        {
            Disconnected,
            InitialHandshake,
            Ok,
            Confused,
            LoggingIn
        }
    }
}
