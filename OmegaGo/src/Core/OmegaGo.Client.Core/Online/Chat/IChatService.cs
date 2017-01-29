using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Chat
{
    public interface IChatService
    {
        event EventHandler<ChatMessage> MessageReceived;
        void SendMessage(string message);
    }
}
