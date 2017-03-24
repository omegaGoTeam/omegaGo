using System;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Timer;

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
        public static IgsConnection Igs
        {
            get
            {
                if (_igsConnection == null)
                {
                    InitIgsConnection();
                }
                return _igsConnection;
            }
        }

        /// <summary>
        /// Gets the connection to KGS Go server. 
        /// </summary>
        public static KgsConnection Kgs

        {
            get
            {
                if (_kgsConnection == null)
                {
                    InitKgsConnection();
                }
                return _kgsConnection;
            }
        }

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

        /// <summary>
        /// Initializes the IGS connection
        /// </summary>
        private static void InitIgsConnection()
        {
            _igsConnection = new IgsConnection();
            _igsConnection.PersonalInformationUpdate += IgsUserUpdate;
            Mvx.Resolve<ITimerService>()
                .StartTimer(TimeSpan.FromSeconds(10), async () => { await _igsConnection.Commands.AreYouThere(); });
        }

        /// <summary>
        /// Initializes the KGS connection
        /// </summary>
        /// <returns></returns>
        private static void InitKgsConnection()
        {
            _kgsConnection = new KgsConnection();
            _kgsConnection.Events.PersonalInformationUpdate += KgsUserUpdate;
            Mvx.Resolve<ITimerService>()
                .StartTimer(TimeSpan.FromSeconds(10), async () => { await _kgsConnection.Commands.WakeUpAsync(); });
        }


        /// <summary>
        /// Handles IGS user update
        /// </summary>
        private static void IgsUserUpdate(object sender, IgsUser user)
        {
            //cache the IGS ranking
            Mvx.Resolve<IGameSettings>().Statistics.IgsRank = user.Rank.Trim();
        }

        /// <summary>
        /// Handles KGS user update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="user"></param>
        private static void KgsUserUpdate(object sender, User user)
        {
            Mvx.Resolve<IGameSettings>().Statistics.KgsRank = user.Rank.Trim();
        }
    }
}
