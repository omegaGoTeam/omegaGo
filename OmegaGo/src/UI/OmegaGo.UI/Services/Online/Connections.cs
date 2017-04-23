using System;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Services.Settings;
using OmegaGo.UI.Services.Timer;
using OmegaGo.UI.ViewModels;

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
            _igsConnection.Events.PersonalInformationUpdate += IgsUserUpdate;
            _igsConnection.Events.IncomingMatchRequest += Pandanet_IncomingMatchRequest; 
            _igsConnection.Events.MatchRequestAccepted += Pandanet_MatchRequestAccepted; 
            Mvx.Resolve<ITimerService>()
                .StartTimer(TimeSpan.FromSeconds(10), async () => { await _igsConnection.Commands.AreYouThere(); });
        }

        private static void Pandanet_MatchRequestAccepted(object sender, Core.Modes.LiveGame.Remote.Igs.IgsGame e)
        {
            Mvx.RegisterSingleton<IGame>(e);
            Mvx.Resolve<ITabProvider>()
                .ShowViewModel(
                    new MvxViewModelRequest(typeof(OnlineGameViewModel), new MvxBundle(),
                        new MvxBundle(), MvxRequestedBy.Unknown), TabNavigationType.NewForegroundTab);
        }

        private static void Pandanet_IncomingMatchRequest(Core.Online.Igs.Structures.IgsMatchRequest obj)
        {
            Mvx.RegisterSingleton<GameCreationBundle.GameCreationBundle>(
                new GameCreation.IgsIncomingMatchRequestBundle(obj));
            var newTab = Mvx.Resolve<ITabProvider>()
                .ShowViewModel(
                    new MvxViewModelRequest(typeof(GameCreationViewModel), new MvxBundle(), new MvxBundle(),
                        MvxRequestedBy.Unknown), TabNavigationType.NewBackgroundTab);
            newTab.IsBlinking = true;
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