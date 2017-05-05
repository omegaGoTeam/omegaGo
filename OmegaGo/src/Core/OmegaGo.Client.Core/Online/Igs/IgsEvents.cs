using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs.Structures;

namespace OmegaGo.Core.Online.Igs
{
    public class IgsEvents : ICommonEvents
    {
        private readonly IgsConnection _igsConnection;

        public IgsEvents(IgsConnection igsConnection)
        {
            this._igsConnection = igsConnection;
        }

        public event EventHandler<bool> LoginComplete;

        public void RaiseLoginComplete(bool success)
        {
            LoginComplete?.Invoke(this, success);
        }

        public event EventHandler Disconnected;

        /// <summary>
        ///     Occurs when the IGS SERVER thinks an event occured that demands the user's attention.
        /// </summary>
        public event Action Beep;

        /// <summary>
        ///     Occurs whenever this client sends a line of text to the IGS SERVER.
        /// </summary>
        public event EventHandler<string> OutgoingLine;

        /// <summary>
        ///     Occurs when we receive information from the server about the logged-in user. This happens during the enumeration
        ///     of all players (because that list includes us).
        /// </summary>
        public event EventHandler<IgsUser> PersonalInformationUpdate;

        /// <summary>
        ///     Occurs when somebody requests to play a game of Go against us on the IGS server.
        /// </summary>
        public event Action<IgsMatchRequest> IncomingMatchRequest;

        /// <summary>
        ///     Occurs when another player named ARGUMENT1 declines a match request we sent them.
        /// </summary>
        public event EventHandler<string> MatchRequestDeclined;

        /// <summary>
        ///     Occurs when our match request is accepted and creates a GAME.
        /// </summary>
        public event EventHandler<IgsGame> MatchRequestAccepted;

        /// <summary>
        ///     Occurs when the opponent in a GAME asks us to let them undo a move
        /// </summary>
        public event EventHandler<GameInfo> UndoRequestReceived;

        /// <summary>
        ///     Occurs when an error message is produced by the server; it should be displayed
        ///     non-modally as a popup balloon.
        /// </summary>
        public event EventHandler<string> ErrorMessageReceived;

        /// <summary>
        ///     Occurs when the opponent in a GAME declines our request to undo a move.
        ///     This will also prevent all further undo's in this game.
        /// </summary>
        public event EventHandler<GameInfo> UndoDeclined;

        /// <summary>
        ///     Occurs when the connection class wants to present a log message to the user using the program, such an incoming
        ///     line. However, some other messages may be passed by this also.
        /// </summary>
        public event EventHandler<string> IncomingLine;

        /// <summary>
        ///     Occurs when the IGS SERVER sends a line, but it's not one of the recognized interrupt messages, and there is no
        ///     current request for which we're expecting a reply.
        /// </summary>
        public event Action<string> UnhandledLine;

        /// <summary>
        ///     Occurs when a player send a message directly to us.
        /// </summary>
        public event Action<string> IncomingChatMessage;

        /// <summary>
        ///     Occurs when any user broadcasts a SHOUT message to all online users that don't have receiving SHOUTs disabled.
        /// </summary>
        public event Action<string> IncomingShoutMessage;

        /// <summary>
        /// Occurs just before a new stage of the IGS login process is entered. The stage that's being entered is given as the argument.
        /// </summary>
        public event EventHandler<IgsLoginPhase> LoginPhaseChanged;

        internal void RaiseLoginPhaseChanged(IgsLoginPhase phase)
        {
            _igsConnection.CurrentLoginPhase = phase;
            LoginPhaseChanged?.Invoke(this, phase);
        }

        internal void RaiseDisconnected()
        {
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        internal void OnBeep()
        {
            Beep?.Invoke();
        }

        internal void OnOutgoingLine(string line)
        {
            OutgoingLine?.Invoke(this, line);
        }

        internal void OnUnhandledLine(string unhandledLine)
        {
            UnhandledLine?.Invoke(unhandledLine);
        }

        internal void OnIncomingMatchRequest(IgsMatchRequest matchRequest)
        {
            IncomingMatchRequest?.Invoke(matchRequest);
        }

        internal void OnMatchRequestDeclined(string playerName)
        {
            MatchRequestDeclined?.Invoke(this, playerName);
        }

        internal void OnMatchRequestAccepted(IgsGame acceptedGame)
        {
            MatchRequestAccepted?.Invoke(this, acceptedGame);
        }

        internal void OnUndoRequestReceived(IgsGameInfo game)
        {
            UndoRequestReceived?.Invoke(this, game);
        }

        internal void OnErrorMessageReceived(string errorMessage)
        {
            ErrorMessageReceived?.Invoke(this, errorMessage);
        }

        internal void OnUndoDeclined(IgsGameInfo game)
        {
            UndoDeclined?.Invoke(this, game);
        }


        internal void OnIncomingLine(string message)
        {
            this._igsConnection.LogBuilder.AppendLine(message);
            IncomingLine?.Invoke(this, message);
        }

        internal void OnPersonalInformationUpdate(IgsUser e)
        {
            PersonalInformationUpdate?.Invoke(this, e);
        }

        internal void OnIncomingChatMessage(string line)
        {
            IncomingChatMessage?.Invoke(line);
        }

        internal void OnIncomingShoutMessage(string line)
        {
            IncomingShoutMessage?.Invoke(line);
        }
    }
}