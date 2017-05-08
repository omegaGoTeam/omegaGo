using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Services.Notifications
{
    /// <summary>
    /// Allows the UI project to submit quick toast-like notifications to the UI.
    /// </summary>
    public interface IAppNotificationService
    {
        /// <summary>
        /// Immediately show a notification on screen.
        /// </summary>
        /// <param name="notification">The notification.</param>
        void TriggerNotification(BubbleNotification notification);
    }
}
