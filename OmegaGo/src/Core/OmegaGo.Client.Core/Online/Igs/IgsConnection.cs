using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OmegaGo.Core.Extensions;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Online;
using OmegaGo.Core.Online.Chat;
using OmegaGo.Core.Online.Igs.Structures;
using Sockets.Plugin;

namespace OmegaGo.Core.Online.Igs
{
    // TODO make it reconnect automatically when connection is interrupted

    /// <summary>
    /// Represents a connection established with the IGS server. This may not necessarily be a persistent TCP connection, but it retains information
    /// about which user is logged in.
    /// </summary>
    public partial class IgsConnection 
    {
        // TODO disconnections are not thread-safe
        // TODO switch prompt mode when necessary

        private string _hostname;
        private int _port;
        private string _username;
        private string _password;
        /// <summary>
        /// Indicates whether the user wants a Telnet connection to the server to be established. 
        /// If this is true but the connection is lost, it should be restarted.
        /// </summary>
        private bool _shouldBeConnected;


        // Internal TCP connection objects   
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

        // Status
        private List<OnlineGameInfo> _gamesInProgressOnIgs = new List<OnlineGameInfo>();
        private readonly List<GameInfo> _gamesBeingObserved = new List<GameInfo>();
        private readonly List<OnlineGame> _gamesYouHaveOpened = new List<OnlineGame>();
        // Internal synchronization management
        private OnlineGame _incomingMovesAreForThisGame;
        private readonly System.Collections.Concurrent.ConcurrentQueue<IgsRequest> _outgoingRequests =
            new System.Collections.Concurrent.ConcurrentQueue<IgsRequest>();
        private IgsRequest _requestInProgress;
        private readonly object _mutex = new object();
        private IgsComposure _composureBackingField = IgsComposure.Disconnected;
        private readonly List<IgsMatchRequest> _incomingMatchRequests = new List<IgsMatchRequest>();
        private IgsComposure _composure
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
        private readonly object _mutexComposureRegained = new object();
        enum IgsComposure
        {
            Disconnected,
            InitialHandshake,
            Ok,
            Confused,
            LoggingIn
        }
        private string _loginError = null;

        private IgsConnection()
        {
        }
        /// <summary>
        /// Do not call this except from the class <see cref="Connections"/>. 
        /// </summary>
        /// <returns></returns>
        internal static IgsConnection CreateConnectionFromConnectionsStaticClass()
        {
            return new IgsConnection();
        }
        /// <summary>
        /// Sends a command to the IGS server without doing any checking and synchronization. 
        /// This should only be used while testing, and never as part of any player-facing game action.
        /// The response to the command will be handled by the main response loop.
        /// </summary>
        /// <param name="command">The command to send to IGS.</param>
        public void DEBUG_SendRawText(string command)
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
            _composure = IgsComposure.LoggingIn;
            _username = username;
            _password = password;
            _loginError = null;
            _streamWriter.WriteLine("login");
            _streamWriter.WriteLine(_username);
            _streamWriter.WriteLine(_password);
            await WaitUntilComposureChangesAsync();
            if (_composure == IgsComposure.Confused)
            {
                await _client.DisconnectAsync();
                _client = null;
                return false;
            }
            if (_loginError != null)
            {
                OnLogEvent("LOGIN ERROR: " + _loginError);
                return false;
            }
            await MakeRequestAsync("toggle quiet true");
            await MakeRequestAsync("toggle newundo true");
            return true;
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
                _composure = IgsComposure.InitialHandshake;
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
            IgsComposure originalComposure = _composure;
            return Task.Run(() =>
            {
                lock (_mutexComposureRegained)
                {
                    while (_composure == originalComposure)
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



        private readonly Regex _regexUser = new Regex(@"42 +([^ ]+) +.* ([A-Za-z-.].{6})  (...)(\*| ) [^/]+/ *[^ ]+ +[^ ]+ +[^ ]+ +[^ ]+ +([^ ]+) default", RegexOptions.None);
        private IgsUser CreateUserFromTelnetLine(string line)
        {
            Match match = _regexUser.Match(line);
            if (!match.Success)
            {
                throw new Exception("IGS SERVER returned invalid user string.");
            }
            /*
             *  1 - Name
             *  2 - Country
             *  3 - Rank
             *  4 - Calculated or self-described rank?
             *  5 - Flags
             * 
             */

            IgsUser user = new IgsUser()
            {
                Name = match.Groups[1].Value,
                Country = match.Groups[2].Value,
                Rank = match.Groups[3].Value.StartsWith(" ") ? match.Groups[3].Value.Substring(1) : match.Groups[3].Value,
                LookingForAGame = match.Groups[5].Value.Contains("!"),
                RejectsRequests = match.Groups[5].Value.Contains("X")
            };
            return user;

        }
        private void HandleIncomingMove(IgsLine igsLine)
        {


            string trim = igsLine.PureLine.Trim();
            if (trim.StartsWith("Game "))
            {
                // TODO
                /*
                string trim2 = trim.Substring("Game ".Length);
                int gameNumber = int.Parse(trim2.Substring(0, trim2.IndexOf(' ')));
                OnlineGameInfo whatGame = _gamesInProgressOnIgs.Find(gm => gm.ServerId == gameNumber);
                if (whatGame == null)
                {
                    whatGame = new ObsoleteGameInfo
                    {
                        ServerId = gameNumber,
                        Server = this
                    };
                    _gamesInProgressOnIgs.Add(whatGame);
                }
                _incomingMovesAreForThisGame = whatGame;
                */
            }
            else if (trim.Contains("Handicap"))
            {
                //  15   0(B): Handicap 3
                int handicapStones = IgsRegex.ParseHandicapMove(igsLine);
                // TODO
                // _incomingMovesAreForThisGame.GameController.HandicapPhase_PlaceIgsHandicap(handicapStones);
            }
            else
            {
                Match match = this._regexMove.Match(trim);
                string moveIndex = match.Groups[1].Value;
                string mover = match.Groups[2].Value;
                string coordinates = match.Groups[3].Value;
                string captures = match.Groups[4].Value;
                StoneColor moverColor = mover == "B" ? StoneColor.Black : StoneColor.White;
                Move move;
                if (coordinates == "Pass")
                {
                    move = Move.Pass(moverColor);
                }
                else
                {
                    move = Move.PlaceStone(moverColor,
                        Position.FromIgsCoordinates(coordinates));
                }
                string[] captureSplit = captures.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string capture in captureSplit)
                {
                    move.Captures.Add(Position.FromIgsCoordinates(capture));
                }
                // TODO
                //_incomingMovesAreForThisGame.AcceptMoveFromInternet(int.Parse(moveIndex), move);
            }
        }



        private readonly Regex _regexMove = new Regex(@"([0-9]+)\((W|B)\): ([^ ]+)(.*)");


        /// <summary>
        /// Enqueues the <paramref name="command"/> to be sent over Telnet to the IGS SERVER,
        /// then asynchronously receives the entirety of the server's response to this command.
        /// </summary>
        /// <param name="command">The command to send over Telnet.</param>
        /// <returns></returns>
        private async Task<IgsResponse> MakeRequestAsync(string command)
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
        private void MakeUnattendedRequest(string command)
        {
            IgsRequest request = new IgsRequest(command) { Unattended = true };
            _outgoingRequests.Enqueue(request);
            ExecuteRequestFromQueue();
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
                if (_composure != IgsComposure.Ok)
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

        #region Chat
        private void HandleIncomingShoutMessage(string line)
        {
            OnIncomingShoutMessage(line);
        }
        private void HandleIncomingChatMessage(string line)
        {
            OnIncomingChatMessage(line);
        }
        /// <summary>
        /// Occurs when a player send a message directly to us.
        /// </summary>
        public event Action<string> IncomingChatMessage;
        private void OnIncomingChatMessage(string line)
        {
            IncomingChatMessage?.Invoke(line);
        }
        /// <summary>
        /// Occurs when any user broadcasts a SHOUT message to all online users that don't have receiving SHOUTs disabled.
        /// </summary>
        public event Action<string> IncomingShoutMessage;
        private void OnIncomingShoutMessage(string line)
        {
            IncomingShoutMessage?.Invoke(line);
        }
        #endregion
        // Interface requirements
        public string ShortName => "IGS";
        public void RefreshBoard(OnlineGame game)
        {
            // TODO
           // MakeUnattendedRequest("moves " + game.Id);
        }

        #region Events

        /// <summary>
        /// Occurs when the IGS SERVER thinks an event occured that demands the user's attention. 
        /// </summary>
        public event Action Beep;
        private void OnBeep()
        {
            Beep?.Invoke();
        }
        /// <summary>
        /// Occurs whenever this client sends a line of text to the IGS SERVER.
        /// </summary>
        public event Action<String> OutgoingLine;
        private void OnOutgoingLine(string line)
        {
            OutgoingLine?.Invoke(line);
        }
        /// <summary>
        /// Occurs when the IGS SERVER sends a line, but it's not one of the recognized interrupt messages, and there is no
        /// current request for which we're expecting a reply.
        /// </summary>
        public event Action<string> UnhandledLine;
        private void OnUnhandledLine(string unhandledLine)
        {
            UnhandledLine?.Invoke(unhandledLine);
        }
        /// <summary>
        /// Occurs when somebody requests to play a game of Go against us on the IGS server.
        /// </summary>
        public event Action<IgsMatchRequest> IncomingMatchRequest;
        private void OnIncomingMatchRequest(IgsMatchRequest matchRequest)
        {
            IncomingMatchRequest?.Invoke(matchRequest);
        }

        /// <summary>
        /// Occurs when another player named ARGUMENT1 declines a match request we sent them.
        /// </summary>
        public event EventHandler<string> MatchRequestDeclined;
        private void OnMatchRequestDeclined(string playerName)
        {
            MatchRequestDeclined?.Invoke(this, playerName);
        }

        /// <summary>
        /// Occurs when our match request is accepted and creates a GAME.
        /// </summary>
        public event EventHandler<OnlineGameInfo> MatchRequestAccepted;
        private void OnMatchRequestAccepted(OnlineGameInfo acceptedGame)
        {
            MatchRequestAccepted?.Invoke(this, acceptedGame);
        }

        /// <summary>
        /// Occurs when an INCOMING CHAT MESSAGE is received from the server that's stored with a GAME we currently have opened.
        /// </summary>
        public event EventHandler<Tuple<OnlineGameInfo, ChatMessage>> IncomingInGameChatMessage;
        private void OnIncomingInGameChatMessage(OnlineGameInfo relevantGame, ChatMessage chatLine)
        {
            IncomingInGameChatMessage?.Invoke(this, new Tuple<OnlineGameInfo, ChatMessage>(relevantGame, chatLine));
        }

        /// <summary>
        /// Occurs when the opponent in a GAME asks us to let them undo a move
        /// </summary>
        public event EventHandler<OnlineGameInfo> UndoRequestReceived;
        private void OnUndoRequestReceived(OnlineGameInfo game)
        {
            UndoRequestReceived?.Invoke(this, game);
        }
        /// <summary>
        /// Occurs when an error message is produced by the server; it should be displayed
        /// non-modally as a popup balloon.
        /// </summary>
        public event EventHandler<string> ErrorMessageReceived;
        private void OnErrorMessageReceived(string errorMessage)
        {
            ErrorMessageReceived?.Invoke(this, errorMessage);
        }

        /// <summary>
        /// Occurs when the server commands us to act as though the last move didn't take place.
        /// </summary>
        public event EventHandler<OnlineGameInfo> LastMoveUndone;
        private void OnLastMoveUndone(OnlineGameInfo whichGame)
        {
            LastMoveUndone?.Invoke(this, whichGame);
        }
        #endregion

        /// <summary>
        /// Occurs when the opponent in a GAME declines our request to undo a move.
        /// This will also prevent all further undo's in this game.
        /// </summary>
        public event EventHandler<OnlineGameInfo> UndoDeclined;
        private void OnUndoDeclined(OnlineGameInfo game)
        {
            UndoDeclined?.Invoke(this, game);
        }

        public event EventHandler<GameScoreEventArgs> GameScoredAndCompleted;
        private void OnGameScoreAndCompleted(OnlineGameInfo gameInfo, float blackScore, float whiteScore)
        {
            // TODO
            /*
            GameScoredAndCompleted?.Invoke(this, new Igs.GameScoreEventArgs(gameInfo, blackScore, whiteScore));
            */
        }
        /// <summary>
        /// Occurs when the connection class wants to present a log message to the user using the program.
        /// </summary>
        public event EventHandler<string> LogEvent;
        protected void OnLogEvent(string message)
        {
            LogEvent?.Invoke(this, message);
        }
    }
    public class GameScoreEventArgs : EventArgs
    {
        public readonly float BlackScore;
        public readonly OnlineGame GameInfo;
        public readonly float WhiteScore;

        public GameScoreEventArgs(OnlineGame gameInfo, float blackScore, float whiteScore)
        {
            this.GameInfo = gameInfo;
            this.BlackScore = blackScore;
            this.WhiteScore = whiteScore;
        }
    }
}
