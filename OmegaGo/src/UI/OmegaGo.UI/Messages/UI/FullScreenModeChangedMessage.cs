using MvvmCross.Plugins.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Messages.UI
{
    public class FullScreenModeChangedMessage : MvxMessage
    {
        public FullScreenModeChangedMessage(object sender, bool isFullScreen ) : base(sender)
        {
            IsFullScreen = isFullScreen;
        }

        public bool IsFullScreen { get; }
    }
}
