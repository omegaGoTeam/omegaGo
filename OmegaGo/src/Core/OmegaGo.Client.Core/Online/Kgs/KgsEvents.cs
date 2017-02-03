using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsEvents : ICommonEvents
    {
        private KgsConnection kgsConnection;

        public event EventHandler<string> SystemMessage;
        public event EventHandler<string> OutgoingRequest;
        public event EventHandler<KgsGame> GameJoined;
        public event EventHandler<JsonResponse> IncomingMessage;
        public event EventHandler<JsonResponse> UnhandledMessage;
        public event EventHandler<string> Disconnection;

        public void RaiseIncomingMessage(JsonResponse message)
        {
            IncomingMessage?.Invoke(this, message);
        }
        public void RaiseUnhandledMessage(JsonResponse message)
        {
            UnhandledMessage?.Invoke(this, message);
        }
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

        public void RaiseGameJoined(KgsGame ongame)
        {
            GameJoined?.Invoke(this, ongame);
        }

        public void RaiseDisconnection(string reason)
        {
            Disconnection?.Invoke(this, reason);
        }
    }
}
