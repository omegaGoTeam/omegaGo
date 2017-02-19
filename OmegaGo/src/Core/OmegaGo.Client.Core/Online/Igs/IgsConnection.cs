using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Connectors.Igs;
using OmegaGo.Core.Modes.LiveGame.Remote.Igs;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Igs.Events;
using OmegaGo.Core.Online.Igs.Structures;
using Sockets.Plugin;

namespace OmegaGo.Core.Online.Igs
{
    // TODO make it reconnect automatically when connection is interrupted

    /// <summary>
    /// Represents a connection established with the IGS server. This may not necessarily be a persistent TCP connection, but it retains information
    /// about which user is logged in.
    /// </summary>
    public partial class IgsConnection : IServerConnection
    {
        // TODO disconnections are not thread-safe
        // TODO switch prompt mode when necessary
        // TODO send "ayt" or something regularly to prevent timeouts
        // TODO OnIncomingResignation should not be public

        /*
         * Synchronization
         */

        /// <summary>
        /// Synchronization mutex
        /// </summary>
        private readonly object _mutex = new object();

        /// <summary>
        /// Composure regained mutex
        /// </summary>
        private readonly object _mutexComposureRegained = new object();

        /// <summary>
        /// Composure
        /// </summary>
        private IgsComposure _composureBackingField = IgsComposure.Disconnected;

        /*
         * Status    
         */

        private readonly Dictionary<int, IgsConnector> _availableConnectors = new Dictionary<int, IgsConnector>();

        /// <summary>
        /// List of games that are being observed
        /// </summary>
        private readonly List<IgsGame> _gamesBeingObserved = new List<IgsGame>();
        /// <summary>
        /// List of games opened
        /// </summary>
        private readonly List<IgsGame> _gamesYouHaveOpened = new List<IgsGame>();

        /// <summary>
        /// Outgoing IGS requests
        /// </summary>
        private readonly System.Collections.Concurrent.ConcurrentQueue<IgsRequest> _outgoingRequests =
            new System.Collections.Concurrent.ConcurrentQueue<IgsRequest>();

        /// <summary>
        /// List of incoming match requests
        /// </summary>
        private readonly List<IgsMatchRequest> _incomingMatchRequests = new List<IgsMatchRequest>();

        /*
         * Game state info
         */

        /// <summary>
        /// Game for which are the incoming moves relevant
        /// </summary>
        private IgsGame _incomingMovesAreForThisGame;

        /// <summary>
        /// Indicates the request currently in progress
        /// </summary>
        private IgsRequest _requestInProgress;

        /// <summary>
        /// Host name
        /// </summary>
        private string _hostname;

        /// <summary>
        /// Port
        /// </summary>
        private int _port;

        /// <summary>
        /// Username
        /// </summary>
        private string _username;

        /// <summary>
        /// Password
        /// </summary>
        private string _password;

        /// <summary>
        /// Indicates whether the user wants a Telnet connection to the server to be established. 
        /// If this is true but the connection is lost, it should be restarted.
        /// </summary>
        private bool _shouldBeConnected;

        /// <summary>
        /// Contains the login error info
        /// </summary>
        private string _loginError = null;

        /*
         * Internal TCP connection objects  
         */

        /// <summary>
        /// The currently established Telnet connection. If it fails, it should automatically restart.
        /// </summary>
        private TcpSocketClient _client;

        /// <summary>
        /// Auto-flushing stream writer that sends commands to the server.
        /// </summary>
        private StreamWriter _streamWriter;

        /// <summary>
        /// This reader receives text lines from the server.
        /// </summary>
        private StreamReader _streamReader;

        /// <summary>
        /// Default IGS connection constructor
        /// </summary>
        public IgsConnection()
        {
            Commands = new IgsCommands(this);
            Events = new IgsEvents(this);
        }

        /// <summary>
        /// Occurs when the IGS SERVER thinks an event occured that demands the user's attention. 
        /// </summary>
        public event Action Beep;

        /// <summary>
        /// Occurs whenever this client sends a line of text to the IGS SERVER.
        /// </summary>
        public event EventHandler<string> OutgoingLine;

        /// <summary>
        /// Occurs when we receive information from the server about the logged-in user. This happens during the enumeration
        /// of all players (because that list includes us).
        /// </summary>
        public event EventHandler<IgsUser> PersonalInformationUpdate;

        /// <summary>
        /// Occurs when somebody requests to play a game of Go against us on the IGS server.
        /// </summary>
        public event Action<IgsMatchRequest> IncomingMatchRequest;

        /// <summary>
        /// Occurs when another player named ARGUMENT1 declines a match request we sent them.
        /// </summary>
        public event EventHandler<string> MatchRequestDeclined;

        /// <summary>
        /// Occurs when our match request is accepted and creates a GAME.
        /// </summary>
        public event EventHandler<IgsGame> MatchRequestAccepted;

        /// <summary>
        /// Occurs when an INCOMING CHAT MESSAGE is received from the server that's stored with a GAME we currently have opened.
        /// </summary>
        public event EventHandler<Tuple<IgsGameInfo, ChatMessage>> IncomingInGameChatMessage;

        /// <summary>
        /// Occurs when the opponent in a GAME asks us to let them undo a move
        /// </summary>
        public event EventHandler<IgsGameInfo> UndoRequestReceived;

        /// <summary>
        /// Occurs when an error message is produced by the server; it should be displayed
        /// non-modally as a popup balloon.
        /// </summary>
        public event EventHandler<string> ErrorMessageReceived;

        /// <summary>
        /// Occurs when the server commands us to act as though the last move didn't take place.
        /// </summary>
        public event EventHandler<IgsGameInfo> LastMoveUndone;

        /// <summary>
        /// Occurs when the opponent in a GAME declines our request to undo a move.
        /// This will also prevent all further undo's in this game.
        /// </summary>
        public event EventHandler<IgsGameInfo> UndoDeclined;

        /// <summary>
        /// Occurs when the game is scored and completed
        /// </summary>
        public event EventHandler<GameScoreEventArgs> GameScoredAndCompleted;

        /// <summary>
        /// Occurs when the connection class wants to present a log message to the user using the program, such an incoming line. However, some other messages may be passed by this also.
        /// </summary>
        public event EventHandler<string> IncomingLine;

        /// <summary>
        /// Occurs when a stone is removed
        /// </summary>
        public event EventHandler<StoneRemovalEventArgs> StoneRemoval;

        /// <summary>
        /// Occurs when the IGS SERVER sends a line, but it's not one of the recognized interrupt messages, and there is no
        /// current request for which we're expecting a reply.
        /// </summary>
        public event Action<string> UnhandledLine;

        /// <summary>
        /// Occurs when a player send a message directly to us.
        /// </summary>
        public event Action<string> IncomingChatMessage;

        /// <summary>
        /// Occurs when any user broadcasts a SHOUT message to all online users that don't have receiving SHOUTs disabled.
        /// </summary>
        public event Action<string> IncomingShoutMessage;

        /// <summary>
        /// Checks if  the connection has been established
        /// </summary>
        public bool ConnectionEstablished => _shouldBeConnected;

        /// <summary>
        /// Checks if the user has been logged in
        /// </summary>
        public bool LoggedIn => ConnectionEstablished && Composure == IgsComposure.Ok;

        /// <summary>
        /// IGS events
        /// </summary>
        public IgsEvents Events { get; }

        public ServerId Name => ServerId.Igs;

        /// <summary>
        /// IGS commands
        /// </summary>
        public IgsCommands Commands { get; }

        /// <summary>
        /// Gets user's username
        /// </summary>
        public string Username => _username;

        /// <summary>
        /// Implements IServerConnection Commands
        /// </summary>
        ICommonCommands IServerConnection.Commands => Commands;

        /// <summary>
        /// Implements IServerConnection Events
        /// </summary>
        ICommonEvents IServerConnection.Events => Events;

        /// <summary>
        /// Provides access to IGS composure, ensures monitor pulsing
        /// </summary>
        private IgsComposure Composure
        {
            get { return _composureBackingField; }
            set
            {
                _composureBackingField = value;
                lock (_mutexComposureRegained)
                {
                    Monitor.PulseAll(_mutexComposureRegained);
                }
            }
        }

        /// <summary>
        /// Sends a command to the IGS server without doing any checking and synchronization. 
        /// This should only be used while testing, and never as part of any player-facing game action.
        /// The response to the command will be handled by the main response loop.
        /// </summary>
        /// <param name="command">The command to send to IGS.</param>
        [Conditional("DEBUG")]
        private void DEBUG_SendRawText(string command)
        {
            _streamWriter.WriteLine(command);
        }

        /// <summary>
        /// Connects to IGS at the specified host and port and logs in as a guest account. If we are already connected,
        /// then this method does nothing.
        /// </summary>
        /// <param name="hostname">The hostname to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(string hostname = ServerLocations.IgsPrimary, int port = ServerLocations.IgsPortPrimary)
        {
            _hostname = hostname;
            _port = port;
            _shouldBeConnected = true;
            try
            {
                await EnsureConnectedAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public async Task DisconnectAsync()
        {
            _shouldBeConnected = false;
            await _client.DisconnectAsync();
            _client = null;
        }

        /// <summary>
        /// Unlike other methods, this is not always thread-safe. 
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public async Task<bool> LoginAsync(string username, string password)
        {
            if (username == null) throw new ArgumentNullException(nameof(username));
            if (password == null) throw new ArgumentNullException(nameof(password));
            await EnsureConnectedAsync();
            Composure = IgsComposure.LoggingIn;
            _username = username;
            _password = password;
            _loginError = null;
            ClearConnectionInformation();
            _streamWriter.WriteLine("login");
            _streamWriter.WriteLine(_username);
            _streamWriter.WriteLine(_password);
            await WaitUntilComposureChangesAsync();
            if (Composure == IgsComposure.Confused)
            {
                await _client.DisconnectAsync();
                _client = null;
                return false;
            }
            if (_loginError != null)
            {
                OnIncomingLine("LOGIN ERROR: " + _loginError);
                return false;
            }
            await MakeRequestAsync("toggle quiet true");
            await MakeRequestAsync("toggle newundo true");
            await MakeRequestAsync("toggle verbose false");
            return true;
        }

        /// <summary>
        /// Enqueues a command to be send to IGS.
        /// </summary>
        /// <param name="command">The single-line command.</param>
        internal void MakeUnattendedRequest(string command)
        {
            IgsRequest request = new IgsRequest(command) { Unattended = true };
            _outgoingRequests.Enqueue(request);
            ExecuteRequestFromQueue();
        }

        /// <summary>
        /// Handles incoming resignation
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="whoResigned">Name of the player who resigned</param>
        internal void HandleIncomingResignation(IgsGameInfo gameInfo, string whoResigned)
        {
            var stoneColor = GetStoneColorForPlayerName(gameInfo.IgsIndex, whoResigned);
            if (stoneColor == StoneColor.None) throw new InvalidOperationException("The player resignation is invalid for this game");
            _availableConnectors[gameInfo.IgsIndex].ResignationFromServer(stoneColor);
            _gamesYouHaveOpened.Remove(_gamesYouHaveOpened.FirstOrDefault(g => g.Info.IgsIndex == gameInfo.IgsIndex));
        }

        /// <summary>
        /// Registers a IGS game connector
        /// </summary>
        /// <param name="connector">Connector</param>
        internal void RegisterConnector(IgsConnector connector)
        {
            if (connector == null) throw new ArgumentNullException(nameof(connector));
            //TODO: Replace the old connector? The index can be reused?
            // (Petr) Right, so, the way it works is this:
            // At a single moment, there can be only one game with an ID on the server. However, as soon as
            // that game ends (for any reason), the server is free to reassign its ID to a newly created game.
            // This does happen in practice, often immediately, because new games are always being created.
            // The IgsConnection class IS catching most of the messages that cause a game to be deleted and
            // if that happens, it is removed from _gamesYouHaveOpened. A game-deletion message should arrive for 
            // all games that we have opened before we receive any information about a new game with the same ID,
            // BUT I'm certainly not sure that I handle all these messages correctly or that I catch all of them.
            // This part of the protocol (and my implementation in this area) is rather messy.
            // (Petr) I'll think about what can be done about this.
            if (_availableConnectors.ContainsKey(connector.GameId)) throw new ArgumentException("This game was already registered", nameof(connector));
            _availableConnectors[connector.GameId] = connector;
        }

        /// <summary>
        /// Enqueues the <paramref name="command"/> to be sent over Telnet to the IGS SERVER,
        /// then asynchronously receives the entirety of the server's response to this command.
        /// </summary>
        /// <param name="command">The command to send over Telnet.</param>
        /// <returns></returns>
        internal async Task<IgsResponse> MakeRequestAsync(string command)
        {
            IgsRequest request = new IgsRequest(command);
            _outgoingRequests.Enqueue(request);
            ExecuteRequestFromQueue();
            IgsResponse lines = await request.GetAllLines();
            lock (_mutex)
            {
                Debug.Assert(_requestInProgress == request);
                _requestInProgress = null;
            }
            ExecuteRequestFromQueue();
            return lines;

        }

        /// <summary>
        /// This method is called whenever a new command request is enqueued to be sent to
        ///  the IGS SERVER and also whenever
        /// a command request becomes completed. The method will determine whether the channel
        ///  is currently free (i.e. no other command is being executed) and
        ///  if so, if there are any command requests in the queue,
        ///  the earliest one is dequeued and executed.
        /// </summary>
        private void ExecuteRequestFromQueue()
        {

            lock (_mutex)
            {
                if (Composure != IgsComposure.Ok)
                {
                    return; // Cannot yet send requests.
                }
                if (_requestInProgress == null)
                {
                    IgsRequest dequeuedItem;
                    if (_outgoingRequests.TryDequeue(out dequeuedItem))
                    {
                        if (!dequeuedItem.Unattended)
                        {
                            _requestInProgress = dequeuedItem;
                        }
                        OnOutgoingLine(dequeuedItem.Command);
                        _streamWriter.WriteLine(dequeuedItem.Command);
                        if (dequeuedItem.Unattended)
                        {
                            ExecuteRequestFromQueue();
                        }
                    }
                }
            }
        }


        private void ClearConnectionInformation()
        {
            IgsRequest throwaway;
            while (_outgoingRequests.TryDequeue(out throwaway))
            {
            }
            _incomingMovesAreForThisGame = null;
            _gamesBeingObserved.Clear();
            _gamesYouHaveOpened.Clear();
            // _gamesInProgressOnIgs.Clear();
        }

        /// <summary>
        /// Verifies that we are currectly connected to the server. If not but we *wish* to be connected,
        /// it attempts to establish the connection. If not and we don't wish to be connected, it fails.
        /// </summary>
        private async Task EnsureConnectedAsync()
        {
            if (_client != null)
            {
                // We are likely to still be connected.
                return;
            }
            if (!_shouldBeConnected)
            {
                throw new Exception("A method was called that requires an IGS connection but the 'Connect()' method was not called; or maybe 'Disconnect()' was called.");
            }
            _client = new TcpSocketClient();
            try
            {
                await _client.ConnectAsync(_hostname, _port);

                _streamWriter = new StreamWriter(_client.WriteStream);
                _streamReader = new StreamReader(_client.ReadStream);
                _streamWriter.AutoFlush = true;
#pragma warning disable 4014
                HandleIncomingData(_streamReader).ContinueWith(t =>
                {
                    // Fail silently.
                }, TaskContinuationOptions.OnlyOnFaulted);
#pragma warning restore 4014
                Composure = IgsComposure.InitialHandshake;
                _streamWriter.WriteLine("guest");
                _streamWriter.WriteLine("toggle client on");
                await WaitUntilComposureChangesAsync();
            }
            catch
            {
                throw new Exception("We failed to establish a connection with the server.");
            }
        }

        private Task WaitUntilComposureChangesAsync()
        {
            IgsComposure originalComposure = Composure;
            return Task.Run(() =>
            {
                lock (_mutexComposureRegained)
                {
                    while (Composure == originalComposure)
                    {
                        Monitor.Wait(_mutexComposureRegained);
                    }
                }
            });
        }

        private IgsCode ExtractCodeFromLine(string line)
        {
            if (line != "")
            {
                string[] split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string firstWord = split[0];
                int firstWordAsInteger;
                if (int.TryParse(firstWord, out firstWordAsInteger))
                {
                    IgsCode code = (IgsCode)firstWordAsInteger;
                    return code;
                }
            }
            return IgsCode.Unknown;
        }

        private StoneColor GetStoneColorForPlayerName(int igsGameIndex, string playerName)
        {
            var game = _gamesYouHaveOpened.Find(og => og.Info.IgsIndex == igsGameIndex);
            var player = game.Controller.Players.FirstOrDefault(p => p.Info.Name == playerName);
            return player?.Info.Color ?? StoneColor.None;
        }

        private void HandleIncomingShoutMessage(string line)
        {
            OnIncomingShoutMessage(line);
        }

        private void HandleIncomingChatMessage(string line)
        {
            OnIncomingChatMessage(line);
        }


        private void OnIncomingChatMessage(string line)
        {
            IncomingChatMessage?.Invoke(line);
        }


        private void OnIncomingShoutMessage(string line)
        {
            IncomingShoutMessage?.Invoke(line);
        }

        private void HandleIncomingMove(IgsGame game, int moveIndex, Move theMove)
        {
            _availableConnectors[game.Info.IgsIndex].MoveFromServer(moveIndex, theMove);
        }

        private void OnIncomingHandicapInformation(IgsGame game, int stoneCount)
        {
            _availableConnectors[game.Info.IgsIndex].HandicapFromServer(stoneCount);
        }

        private void OnBeep()
        {
            Beep?.Invoke();
        }

        private void OnOutgoingLine(string line)
        {
            OutgoingLine?.Invoke(this, line);
        }

        private void OnUnhandledLine(string unhandledLine)
        {
            UnhandledLine?.Invoke(unhandledLine);
        }

        private void OnIncomingMatchRequest(IgsMatchRequest matchRequest)
        {
            IncomingMatchRequest?.Invoke(matchRequest);
        }

        private void OnMatchRequestDeclined(string playerName)
        {
            MatchRequestDeclined?.Invoke(this, playerName);
        }

        private void OnMatchRequestAccepted(IgsGame acceptedGame)
        {
            MatchRequestAccepted?.Invoke(this, acceptedGame);
        }

        /// <summary>
        /// Fires incoming in-game chat message
        /// </summary>
        private void OnIncomingInGameChatMessage(IgsGameInfo relevantGame, ChatMessage chatLine)
        {
            IncomingInGameChatMessage?.Invoke(this, new Tuple<IgsGameInfo, ChatMessage>(relevantGame, chatLine));
        }

        private void OnUndoRequestReceived(IgsGameInfo game)
        {
            UndoRequestReceived?.Invoke(this, game);
        }

        private void OnErrorMessageReceived(string errorMessage)
        {
            ErrorMessageReceived?.Invoke(this, errorMessage);
        }

        private void OnLastMoveUndone(IgsGameInfo whichGame)
        {
            LastMoveUndone?.Invoke(this, whichGame);
        }

        private void OnUndoDeclined(IgsGameInfo game)
        {
            UndoDeclined?.Invoke(this, game);
        }

        private void OnGameScoreAndCompleted(IgsGame gameInfo, float blackScore, float whiteScore)
        {
            GameScoredAndCompleted?.Invoke(this, new GameScoreEventArgs(gameInfo, blackScore, whiteScore));
        }


        private void OnIncomingLine(string message)
        {
            IncomingLine?.Invoke(this, message);
        }

        private void OnPersonalInformationUpdate(IgsUser e)
        {
            PersonalInformationUpdate?.Invoke(this, e);
        }

        private void OnIncomingStoneRemoval(int gameNumber, Position deadPosition)
        {
            var game = _gamesYouHaveOpened.Find(og => og.Info.IgsIndex == gameNumber);
            StoneRemoval?.Invoke(this, new StoneRemovalEventArgs(game, deadPosition));
        }
    }
}
