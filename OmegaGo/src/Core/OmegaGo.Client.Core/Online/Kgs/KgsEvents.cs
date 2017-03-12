using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs.Datatypes;

namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// Use events in this class to receive information from KGS. The Raise* message (such as <see cref="RaiseGameJoined(KgsGame)"/>) should only be called by classes within the <see cref="OmegaGo.Core.Online.Kgs"/> namespace.  
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.Common.ICommonEvents" />
    public class KgsEvents
    {
        private readonly KgsConnection _kgsConnection;
        
        public KgsEvents(KgsConnection kgsConnection)
        {
            _kgsConnection = kgsConnection;
        }

        public event EventHandler<string> SystemMessage;
        public event EventHandler<string> OutgoingRequest;
        public event EventHandler<KgsGame> GameJoined;
        public event EventHandler<JsonResponse> IncomingMessage;
        public event EventHandler<JsonResponse> UnhandledMessage;
        public event EventHandler<string> Disconnection;
        public event EventHandler<User> PersonalInformationUpdate;

        public void RaisePersonalInformationUpdate(User you)
        {
            PersonalInformationUpdate?.Invoke(this, you);
        }
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
