using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// Your password did not match.
    /// </summary>
    /// <seealso cref="KgsInterruptResponse" />
    class LoginFailedBadPassword : KgsInterruptResponse
    {
        public override void Process(KgsConnection connection)
        {
            connection.LoggingIn = false;
            connection.Events.RaiseLoginComplete(LoginResult.FailureWrongPassword);
        }
    }
}
