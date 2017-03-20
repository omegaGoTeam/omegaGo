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
        public static async void Startup()
        {
            IGameSettings settings = Mvx.Resolve<IGameSettings>();
            var notifications = Mvx.Resolve<IAppNotificationService>();
            if (settings.Interface.IgsAutoLogin && settings.Interface.IgsRememberPassword)
            {
                if (await Connections.Igs.ConnectAsync())
                {
                    var success =
                        await Connections.Igs.LoginAsync(settings.Interface.IgsName, settings.Interface.IgsPassword);
                    if (success)
                    {
                        notifications.TriggerNotification(new BubbleNotification("IGS connection ok."));
                    }
                }
            }
            if (settings.Interface.KgsAutoLogin && settings.Interface.KgsRememberPassword)
            {
                var success =
                    await Connections.Kgs.LoginAsync(settings.Interface.KgsName, settings.Interface.KgsPassword);
                if (!success)
                {

                }
            }
        }
    }
}
