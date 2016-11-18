using MvvmCross.Plugins.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Messages.Game
{
    public sealed class RedrawBoardMessage : MvxMessage
    {
        public RedrawBoardMessage(object sender) 
            : base(sender)
        {

        }
    }
}
