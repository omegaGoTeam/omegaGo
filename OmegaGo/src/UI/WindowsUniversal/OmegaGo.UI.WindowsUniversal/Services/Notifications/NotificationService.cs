using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Services.Notifications;
using OmegaGo.UI.WindowsUniversal.Infrastructure;

namespace OmegaGo.UI.WindowsUniversal.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        public void TriggerNotification(BubbleNotification notification)
        {
            AppShell.GetForCurrentView().TriggerBubbleNotification(notification);
        }
    }
}
