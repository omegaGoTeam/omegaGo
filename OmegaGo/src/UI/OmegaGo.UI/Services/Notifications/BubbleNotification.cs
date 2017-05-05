using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.UserControls.ViewModels;

namespace OmegaGo.UI.Services.Notifications
{
    /// <summary>
    /// In-app bubble notification 
    /// </summary>
    public class BubbleNotification : ControlViewModelBase
    {
        /// <summary>
        /// Creates a bubble notification
        /// </summary>
        /// <param name="text">Text of the notification</param>
        /// <param name="heading">Heading of the notification</param>
        /// <param name="type">Type of the notification</param>
        public BubbleNotification(string text, string heading = null, NotificationType type = NotificationType.Info)
        {
            Text = text;
            Heading = heading;
            Type = type;
        }

        /// <summary>
        /// Heading of the notification
        /// </summary>
        public string Heading { get; }

        /// <summary>
        /// Text of the notification
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Time when the notification first appeared
        /// </summary>
        public DateTime FirstAppeared { get; set; }

        /// <summary>
        /// Type of the notification
        /// </summary>
        public NotificationType Type { get; }
    }
}
