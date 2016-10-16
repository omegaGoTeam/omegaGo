using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online
{
    public abstract class ServerConnection
    {
        /// <summary>
        /// Asks the server to send some information to tell us that it's alive.
        /// </summary>
        /// <returns>Some information from the server.</returns>
        public virtual string Hello()
        {
            throw new Exception("This method was not overriden.");
        }

        /// <summary>
        /// Attempts to log in the user to the server associated with this class. If is succeeds, the class instance remembers the login data and establishes 
        /// the connection.
        /// </summary>
        /// <param name="username">User's login name</param>
        /// <param name="password">User's password.</param>
        /// <returns>True if the server permitted the login.</returns>
        public abstract bool Login(string username, string password);


        /// <summary>
        /// Sends a log message that should be displayed to the user using the program.
        /// </summary>
        public event Action<string> LogEvent;

        protected void OnLogEvent(string message)
        {
            LogEvent?.Invoke(message);
        }
    }
}
