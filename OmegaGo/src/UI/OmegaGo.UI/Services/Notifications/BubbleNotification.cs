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
        public BubbleNotification(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Text of the notification
        /// </summary>
        public string Text { get; }

        public DateTime FirstAppeared { get; set; }
    }
}
