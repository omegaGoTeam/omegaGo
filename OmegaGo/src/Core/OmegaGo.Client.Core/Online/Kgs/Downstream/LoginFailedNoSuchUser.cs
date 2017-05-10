using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    /// <summary>
    /// Your user name is not on the server.
    /// </summary>
    /// <seealso cref="KgsInterruptResponse" />
    class LoginFailedNoSuchUser : KgsInterruptResponse
    {
        public override void Process(KgsConnection connection)
        {
            connection.LoggingIn = false;
            connection.Events.RaiseLoginComplete(LoginResult.FailureUserDoesNotExist);
        }
    }
}
