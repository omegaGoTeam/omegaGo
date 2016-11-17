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
using OmegaGo.Core.Online.Igs.Structures;
using Sockets.Plugin;

namespace OmegaGo.Core.Online.Igs
{
    // TODO make it reconnect automatically when connection is interrupted


    /// <summary>
    /// Represents a connection established with the IGS server. This may not necessarily be a persistent TCP connection, but it retains information
    /// about which user is logged in.
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.ServerConnection" />
    public partial class IgsConnection : ServerConnection
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
        private List<Game> _gamesInProgressOnIgs = new List<Game>();
        private readonly List<Game> _gamesBeingObserved = new List<Game>();
        // Internal synchronization management
        private Game _incomingMovesAreForThisGame;
        private readonly System.Collections.Concurrent.ConcurrentQueue<IgsRequest> _outgoingRequests =
            new System.Collections.Concurrent.ConcurrentQueue<IgsRequest>();
        private IgsRequest _requestInProgress;
        private readonly object _mutex = new object();
        private IgsComposure _composureBackingField = IgsComposure.Disconnected;
        public List<IgsMatchRequest> IncomingMatchRequests = new List<IgsMatchRequest>();
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
        public async Task<bool> Connect(string hostname = ServerLocations.IgsPrimary, int port = ServerLocations.IgsPortPrimary)
        {
            _hostname = hostname;
            _port = port;
            _shouldBeConnected = true;
            try
            {
                await EnsureConnected();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public async Task Disconnect()
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
        public async Task<bool> Login(string username, string password)
        {
            if (username == null) throw new ArgumentNullException(nameof(username));
            if (password == null) throw new ArgumentNullException(nameof(password));
            await EnsureConnected();
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
            await MakeRequest("toggle quiet true");
            return true;
        }

        /// <summary>
        /// Verifies that we are currectly connected to the server. If not but we *wish* to be connected,
        /// it attempts to establish the connection. If not and we don't wish to be connected, it fails.
        /// </summary>
        private async Task EnsureConnected()
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
                HandleIncomingData(_streamReader).ContinueWith(t => {
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
                string trim2 = trim.Substring("Game ".Length);
                int gameNumber = int.Parse(trim2.Substring(0, trim2.IndexOf(' ')));
                Game whatGame = _gamesInProgressOnIgs.Find(gm => gm.ServerId == gameNumber);
                if (whatGame == null)
                {
                    whatGame = new Game
                    {
                        ServerId = gameNumber,
                        Server = this
                    };
                    _gamesInProgressOnIgs.Add(whatGame);
                }
                _incomingMovesAreForThisGame = whatGame;
            }
            else if (trim.Contains("Handicap"))
            {
                //  15   0(B): Handicap 3
                // Ignore.
            }
            else
            {
                Match match = regexMove.Match(trim);
                string moveIndex = match.Groups[1].Value;
                string mover = match.Groups[2].Value;
                string coordinates = match.Groups[3].Value;
                string captures = match.Groups[4].Value;
                Move move = Move.PlaceStone(mover == "B" ? StoneColor.Black : StoneColor.White,
                    Position.FromIgsCoordinates(coordinates));
                string[] captureSplit = captures.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string capture in captureSplit)
                {
                    move.Captures.Add(Position.FromIgsCoordinates(capture));
                }
                _incomingMovesAreForThisGame.AcceptMoveFromInternet(int.Parse(moveIndex) + 1, move);
            }
        }

     

        private readonly Regex regexMove = new Regex(@"([0-9]+)\((W|B)\): ([^ ]+)(.*)");


        /// <summary>
        /// Enqueues the <paramref name="command"/> to be sent over Telnet to the IGS SERVER,
        /// then asynchronously receives the entirety of the server's response to this command.
        /// </summary>
        /// <param name="command">The command to send over Telnet.</param>
        /// <returns></returns>
        private async Task<List<IgsLine>> MakeRequest(string command)
        {
            IgsRequest request = new IgsRequest(command);
            _outgoingRequests.Enqueue(request);
            ExecuteRequestFromQueue();
            List<IgsLine> lines = await request.GetAllLines();
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
            IgsRequest request = new IgsRequest(command) {Unattended = true};
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



        // Interface requirements
        public override string ShortName => "IGS";
        public void RefreshBoard(Game game)
        {
            MakeUnattendedRequest("moves " + game.ServerId);
        }

       
    }
}
