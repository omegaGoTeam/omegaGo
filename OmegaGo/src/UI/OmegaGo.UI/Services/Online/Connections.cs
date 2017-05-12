using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Kgs;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.UI.Infrastructure.Tabbed;
using OmegaGo.UI.Localization;
using OmegaGo.UI.Services.Audio;
using OmegaGo.UI.Services.GameCreation;
using OmegaGo.UI.Services.Notifications;
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
            _igsConnection.Events.MatchRequestDeclined += Pandanet_MatchRequestDeclined;
            _igsConnection.Events.ErrorMessageReceived += Pandanet_ErrorMessageReceived;
            Mvx.Resolve<ITimerService>()
                .StartTimer(TimeSpan.FromSeconds(10), async () => { await _igsConnection.Commands.AreYouThere(); });
        }

        private static void Pandanet_ErrorMessageReceived(object sender, string e)
        {
            Mvx.Resolve<IAppNotificationService>()
                .TriggerNotification(new BubbleNotification(e, LocalizedStrings.PandanetError, NotificationType.Alert));

        }

        private static void Pandanet_MatchRequestDeclined(object sender, string e)
        {
            Mvx.Resolve<IAppNotificationService>()
                .TriggerNotification(new BubbleNotification(String.Format(LocalizedStrings.XDeclinedYourMatchRequest, e), null, NotificationType.Alert));
        }

        private static void Pandanet_MatchRequestAccepted(object sender, Core.Modes.LiveGame.Remote.Igs.IgsGame e)
        {
            Mvx.RegisterSingleton<IGame>(e);
            Mvx.Resolve<ITabProvider>()
                .ShowViewModel(
                    new MvxViewModelRequest(typeof(OnlineGameViewModel), new MvxBundle(),
                        new MvxBundle(), MvxRequestedBy.Unknown), TabNavigationType.NewForegroundTab);
        }

        private static async void Pandanet_IncomingMatchRequest(Core.Online.Igs.Structures.IgsMatchRequest obj)
        {
            Mvx.RegisterSingleton<GameCreation.GameCreationBundle>(
                new GameCreation.IgsIncomingMatchRequestBundle(obj));
            CreateTab<GameCreationViewModel>(TabNavigationType.NewBackgroundTab);

            var settings = Mvx.Resolve<IGameSettings>();
            if (settings.Audio.PlayWhenNotificationReceived)
            {
                await Sounds.IncomingMatchRequest.PlayAsync();
            }
        }

        /// <summary>
        /// Initializes the KGS connection
        /// </summary>
        /// <returns></returns>
        private static void InitKgsConnection()
        {
            _kgsConnection = new KgsConnection();
            _kgsConnection.Events.PersonalInformationUpdate += KgsUserUpdate;
            _kgsConnection.Events.GameJoined += Kgs_GameJoined;
            _kgsConnection.Events.NotificationErrorMessage += KgsNotificationErrorMessage;
            _kgsConnection.Events.ChallengeJoined += Kgs_ChallengeJoined;
            Mvx.Resolve<ITimerService>()
                .StartTimer(TimeSpan.FromSeconds(10), async () => { await _kgsConnection.Commands.WakeUpAsync(); });
        }
        

        private static void Kgs_ChallengeJoined(object sender, Core.Online.Kgs.Structures.KgsChallenge e)
        {
            if (e.OwnedByUs)
            {
                Mvx.RegisterSingleton<GameCreationBundle>(new KgsChallengeManagementBundle(e));
            }
            else
            {
                Mvx.RegisterSingleton<GameCreationBundle>(new KgsJoinChallengeBundle(e));
            }
            CreateTab<GameCreationViewModel>(TabNavigationType.NewForegroundTab);
        }

        private static void KgsNotificationErrorMessage(object sender, string message)
        {
            string title = LocalizedStrings.KGSTechnicalMessage;
            switch (message)
            {
                case "CHANNEL_ALREADY_JOINED":
                    title = LocalizedStrings.KGSAlert;
                    message = LocalizedStrings.KGSChannelAlreadyJoined;
                    break;
                case "RECONNECT":
                    title = LocalizedStrings.KGSAlert;
                    message = LocalizedStrings.KGSReconnect;
                    break;
                case "CANT_PLAY_TWICE":
                    title = LocalizedStrings.KGSAlert;
                    message = LocalizedStrings.KGSYouCantPlayTwice;
                    break;
                case "CHALLENGE_CANT_PLAY_RANKED":
                    title = LocalizedStrings.KGSAlert;
                    message = LocalizedStrings.CantPlayRanked;
                    break;
            }
            Mvx.Resolve<IAppNotificationService>()
                .TriggerNotification(new BubbleNotification(message, title, NotificationType.Alert));
        }

        private static void Kgs_GameJoined(object sender, KgsGame e)
        {
            Mvx.RegisterSingleton<IGame>(e);
            var tabProvider = Mvx.Resolve<ITabProvider>();
            if (e.Controller.Players.Any(pl => pl.IsLocal))
            {
                CreateTab<OnlineGameViewModel>(TabNavigationType.NewForegroundTab);
            }
            else
            {
                CreateTab<ObserverGameViewModel>(TabNavigationType.NewForegroundTab);
            }
        }

        private static void CreateTab<T>(TabNavigationType navigationType)
        {
            var newTab = Mvx.Resolve<ITabProvider>()
               .ShowViewModel(
                   new MvxViewModelRequest(typeof(T), new MvxBundle(), new MvxBundle(),
                       MvxRequestedBy.Unknown), navigationType);
            newTab.IsBlinking = navigationType == TabNavigationType.NewBackgroundTab;
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