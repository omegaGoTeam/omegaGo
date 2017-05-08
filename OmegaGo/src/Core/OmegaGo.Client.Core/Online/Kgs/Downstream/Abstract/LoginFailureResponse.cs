using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs.Downstream.Abstract
{
    /// <summary>
    /// Base class for downstream messages sent in response to a failed login attempt.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Kgs.Downstream.Abstract.KgsInterruptResponse" />
    abstract class LoginFailureResponse : KgsInterruptResponse
    {
        public override void Process(KgsConnection connection)
        {
            connection.LoggingIn = false;
            connection.Events.RaiseLoginComplete(false);
        }
    }
}
