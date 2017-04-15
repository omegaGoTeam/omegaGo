using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.Services.Online;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.WindowsUniversal.Services.Uncategorized
{
    class OnlineStartup
    {
        /// <summary>
        /// Starts the login process for all servers where the user wants to login at startup. All login processes
        /// run simultaneously.
        /// </summary>
        public static void Startup()
        {
            IGameSettings settings = Mvx.Resolve<IGameSettings>();
            var notifications = Mvx.Resolve<IAppNotificationService>();
            StartupIgs(settings, notifications);
            StartupKgs(settings, notifications);
        }

        private static async void StartupIgs(IGameSettings settings, IAppNotificationService notifications)
        {
            if (settings.Interface.IgsAutoLogin && settings.Interface.IgsRememberPassword)
            {
                if (await Connections.Igs.ConnectAsync())
                {
                    var success =
                        await Connections.Igs.LoginAsync(settings.Interface.IgsName, settings.Interface.IgsPassword);
                    if (!success)
                    {
                        notifications.TriggerNotification(new BubbleNotification("IGS auto-login failed."));
                    }
                }
            }
        }

        private static async void StartupKgs(IGameSettings settings, IAppNotificationService notifications)
        {

            if (settings.Interface.KgsAutoLogin && settings.Interface.KgsRememberPassword)
            {
                var success =
                    await Connections.Kgs.LoginAsync(settings.Interface.KgsName, settings.Interface.KgsPassword);
                if (!success)
                {
                    notifications.TriggerNotification(new BubbleNotification("KGS auto-login failed."));
                }
            }
        }
    }
}
