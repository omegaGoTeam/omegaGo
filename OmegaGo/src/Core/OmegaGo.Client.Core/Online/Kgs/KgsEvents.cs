using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Remote.Kgs;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Structures;

namespace OmegaGo.Core.Online.Kgs
{
    /// <summary>
    /// Use events in this class to receive information from KGS. The Raise* message (such as <see cref="RaiseGameJoined(KgsGame)"/>) should only be called by classes within the <see cref="OmegaGo.Core.Online.Kgs"/> namespace.  
    /// </summary>    
    public class KgsEvents : ICommonEvents
    {

        public event EventHandler<string> SystemMessage;


        public event EventHandler<KgsChallenge> ChallengeJoined;
        public event EventHandler<string> OutgoingRequest;
        public event EventHandler<KgsGame> GameJoined;
        public event EventHandler<KgsChannel> Unjoin;
        public event EventHandler<JsonResponse> IncomingMessage;
        public event EventHandler<JsonResponse> UnhandledMessage;
        public event EventHandler<string> Disconnection;
        public event EventHandler<string> NotificationErrorMessage;
        public event EventHandler<User> PersonalInformationUpdate;
        public event EventHandler<KgsLoginPhase> LoginPhaseChanged;
        public event EventHandler<GameInfo> UndoRequestReceived;
        public event EventHandler<LoginResult> LoginEnded;

#pragma warning disable CS0067 // KGS does not have a command for declining undos. To decline an undo, you're supposed to just ignore it.
        public event EventHandler<GameInfo> UndoDeclined;
#pragma warning restore CS0067
        public event Action SomethingChanged;
        public event Action<KgsChannel> ChannelJoined;
        public event Action<KgsChannel> ChannelUnjoined;

        internal void RaiseChallengeJoined(KgsChallenge createdChallenge)
        {
            ChallengeJoined?.Invoke(this, createdChallenge);
        }
        internal void RaiseLoginPhaseChanged(KgsLoginPhase phase)
        {
            LoginPhaseChanged?.Invoke(this, phase);
        }

        internal void RaisePersonalInformationUpdate(User you)
        {
            PersonalInformationUpdate?.Invoke(this, you);
        }

        internal void RaiseIncomingMessage(JsonResponse message)
        {
            IncomingMessage?.Invoke(this, message);
        }

        internal void RaiseUnhandledMessage(JsonResponse message)
        {
            UnhandledMessage?.Invoke(this, message);
        }

        internal void RaiseSystemMessage(string logmessage)
        {
            SystemMessage?.Invoke(this, logmessage);
        }

        internal void RaiseOutgoingRequest(string request)
        {
            OutgoingRequest?.Invoke(this, request);
        }

        internal void RaiseLoginComplete(LoginResult result)
        {
            LoginEnded?.Invoke(this, result);
        }

        internal void RaiseGameJoined(KgsGame ongame)
        {
            GameJoined?.Invoke(this, ongame);
        }

        internal void RaiseDisconnection(string reason)
        {
            Disconnection?.Invoke(this, reason);
        }
        internal void RaiseUnjoin(KgsChannel channel)
        {
            Unjoin?.Invoke(this, channel);
        }

        public void RaiseErrorNotification(string type)
        {
            NotificationErrorMessage?.Invoke(this, type);
        }

        internal void RaiseUndoRequestReceived(KgsGame kgsGame)
        {
            UndoRequestReceived?.Invoke(this, kgsGame.Info);
        }

        internal void RaiseSomethingChanged()
        {
            SomethingChanged?.Invoke();
        }

        internal void RaiseChannelJoined(KgsChannel channel)
        {
            ChannelJoined?.Invoke(channel);
        }
        internal void RaiseChannelUnjoined(KgsChannel channel)
        {
            ChannelUnjoined?.Invoke(channel);
        }
    }
}
