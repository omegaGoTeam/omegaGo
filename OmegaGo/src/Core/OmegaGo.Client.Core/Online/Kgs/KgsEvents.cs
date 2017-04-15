﻿using System;
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
        public event EventHandler<string> OutgoingRequest;
        public event EventHandler<KgsGame> GameJoined;
        public event EventHandler<KgsChannel> Unjoin;
        public event EventHandler<JsonResponse> IncomingMessage;
        public event EventHandler<JsonResponse> UnhandledMessage;
        public event EventHandler<string> Disconnection;
        public event EventHandler<string> NotificationMessage;
        public event EventHandler<User> PersonalInformationUpdate;
        public event EventHandler<KgsLoginPhase> LoginPhaseChanged;
        public event EventHandler<GameInfo> UndoRequestReceived;

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

        public void RaiseNotification(string type)
        {
            NotificationMessage?.Invoke(this, type);
        }
    }
}
