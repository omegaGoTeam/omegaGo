using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Igs;

namespace OmegaGo.Core.Online
{
    public static class Connections
    {
        private static IgsConnection _connection;

        /// <summary>
        /// Gets the connection to Pandanet-IGS Go server. 
        /// </summary>
        public static IgsConnection Pandanet
        {
            get
            {
                if (_connection == null)
                {
                    _connection = IgsConnection.CreateConnectionFromConnectionsStaticClass();
                }
                return _connection;
            }
        }

        public static IgsConnection GetConnection(ServerID id)
        {
            if (id == ServerID.Igs)
                return Pandanet;
            throw new Exception("That server does not exist.");
        }
    }
}
