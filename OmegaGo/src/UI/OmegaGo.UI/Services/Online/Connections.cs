﻿using System;
using System.Text;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Kgs;

namespace OmegaGo.UI.Services.Online
{
    /// <summary>
    /// This class holds a single connection instance to each online server. These can be used throughout
    /// the app.
    /// </summary>
    public static class Connections
    {
        private static IgsConnection _igsConnection;
        private static KgsConnection _kgsConnection;

        /// <summary>
        /// Gets the connection to Pandanet-IGS Go server. 
        /// </summary>
        /// <summary>
        /// Gets the connection to Pandanet-IGS Go server. 
        /// </summary>
        public static IgsConnection Igs => _igsConnection ?? 
                                            (_igsConnection = new IgsConnection());

        /// <summary>
        /// Gets the connection to KGS Go server. 
        /// </summary>
        public static KgsConnection Kgs => _kgsConnection ??
                                           (_kgsConnection = new KgsConnection());



        /// <summary>
        /// Gets the connection to the specified server.
        /// </summary>
        /// <param name="id">Identification of the server</param>
        /// <returns></returns>
        public static IServerConnection GetConnection(ServerId id)
        {
            if (id == ServerId.Igs)
                return Igs;
            if (id == ServerId.Kgs)
                return Kgs;
            throw new Exception("That server does not exist.");
        }

    }
}
