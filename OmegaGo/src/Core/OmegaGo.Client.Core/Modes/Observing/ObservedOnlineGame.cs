using OmegaGo.Core.Online.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Modes.LiveGame.Online.Observing
{
    class ObservedOnlineGame : IMode
    {
        IChatService PlayerChat { get; }
        IChatService ObserverChat { get; }
    }
}
