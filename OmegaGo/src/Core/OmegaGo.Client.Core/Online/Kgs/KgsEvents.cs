using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsEvents
    {
        private KgsConnection kgsConnection;

        public event EventHandler<string> SystemMessage;
        public event EventHandler<string> OutgoingRequest;

        public void RaiseSystemMessage(string logmessage)
        {
            SystemMessage?.Invoke(this, logmessage);
        }
        public void RaiseOutgoingRequest(string request)
        {
            OutgoingRequest?.Invoke(this, request);
        }

        public KgsEvents(KgsConnection kgsConnection)
        {
            this.kgsConnection = kgsConnection;
        }
    }
}
