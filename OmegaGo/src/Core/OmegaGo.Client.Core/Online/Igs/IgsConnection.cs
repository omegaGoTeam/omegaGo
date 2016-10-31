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
        private List<Game> _gamesInProgressOnIgs;
        private List<Game> _gamesBeingObserved = new List<Game>();
        // Internal synchronization management
        private Game _incomingMovesAreForThisGame;
        private System.Collections.Concurrent.ConcurrentQueue<IgsRequest> _outgoingRequests =
            new System.Collections.Concurrent.ConcurrentQueue<IgsRequest>();
        private IgsRequest _requestInProgress;
        private object _mutex = new object();
        private IgsComposure _composureBackingField = IgsComposure.Disconnected;
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
        private object _mutexComposureRegained = new object();
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
            this._streamWriter.WriteLine(command);
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
            this._hostname = hostname;
            this._port = port;
            this._shouldBeConnected = true;
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
            this._shouldBeConnected = false;
            await this._client.DisconnectAsync();
            this._client = null;
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
            this._composure = IgsComposure.LoggingIn;
            this._username = username;
            this._password = password;
            this._loginError = null;
            this._streamWriter.WriteLine("login");
            this._streamWriter.WriteLine(this._username);
            this._streamWriter.WriteLine(this._password);
            await WaitUntilComposureChangesAsync();
            if (_composure == IgsComposure.Confused)
            {
                await this._client.DisconnectAsync();
                this._client = null;
                return false;
            }
            if (_loginError != null)
            {
                OnLogEvent("LOGIN ERROR: " + _loginError);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Verifies that we are currectly connected to the server. If not but we *wish* to be connected,
        /// it attempts to establish the connection. If not and we don't wish to be connected, it fails.
        /// </summary>
        private async Task EnsureConnected()
        {
            if (this._client != null)
            {
                // We are likely to still be connected.
                return;
            }
            if (!this._shouldBeConnected)
            {
                throw new Exception("A method was called that requires an IGS connection but the 'Connect()' method was not called; or maybe 'Disconnect()' was called.");
            }
            this._client = new TcpSocketClient();
            try
            {
                await this._client.ConnectAsync(_hostname, _port);

                this._streamWriter = new StreamWriter(this._client.WriteStream);
                this._streamReader = new StreamReader(this._client.ReadStream);
                this._streamWriter.AutoFlush = true;
                HandleIncomingData(this._streamReader);
                this._composure = IgsComposure.InitialHandshake;
                this._streamWriter.WriteLine("guest");
                this._streamWriter.WriteLine("toggle client on");
                await this.WaitUntilComposureChangesAsync();
            }
            catch
            {
                throw new Exception("We failed to establish a connection with the server.");
            }
        }

        private Task WaitUntilComposureChangesAsync()
        {
            IgsComposure originalComposure = this._composure;
            return Task.Run(() =>
            {
                lock (_mutexComposureRegained)
                {
                    while (this._composure == originalComposure)
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



        private Regex _regexUser = new Regex(@"42 +([^ ]+) +.* ([A-Za-z-.].{6})  (...)(\*| ) [^/]+/ *[^ ]+ +[^ ]+ +[^ ]+ +[^ ]+ +([^ ]+) default", RegexOptions.None);
        private IgsUser CreateUserFromTelnetLine(string line)
        {
            Match match = this._regexUser.Match(line);
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
        private async void HandleIncomingData(StreamReader sr)
        {
            bool weAreHandlingAnInterruptMessage = false;
            while (true)
            {
                string line = await sr.ReadLineAsync();
                if (line == null)
                {
                    OnLogEvent("The connection has been terminated."); 
                    // TODO add thread safety
                    this._client = null;
                    return;
                }
                line = line.Trim();

                // IGS occasionally sends blank lines, I don't know why. They serve no reason.
                if (line == "") continue;

                IgsCode code = ExtractCodeFromLine(line);
                IgsLine igsLine = new IgsLine(code, line);
                OnLogEvent(line);

                switch (_composure)
                {
                    case IgsComposure.Confused:
                    case IgsComposure.Ok:
                    case IgsComposure.Disconnected:
                        // No special mode.
                        break;
                    case IgsComposure.InitialHandshake:
                        if (igsLine.EntireLine.Trim() == "1 5")
                        {
                            _composure = IgsComposure.Ok;
                            continue;
                        }
                        else
                        {
                            // Ignore.
                            continue;
                        }
                    case IgsComposure.LoggingIn:
                        if (igsLine.EntireLine.Contains("Invalid password."))
                        {
                            _loginError = "The password is incorrect.";
                        }
                        if (igsLine.EntireLine.Contains("This is a guest account."))
                        {
                            _loginError = "The username does not exist.";
                        }
                        if (igsLine.EntireLine.Contains("1 5"))
                        {
                            _composure = IgsComposure.Ok;
                            continue;
                        }
                        break;

                }

                if (weAreHandlingAnInterruptMessage && code == IgsCode.Prompt)
                {
                    // Interrupt message is over, let's wait for a new message
                    weAreHandlingAnInterruptMessage = false;
                    continue;
                }
                if (code == IgsCode.Beep)
                {
                    OnBeep();
                    continue;
                }
                if (code == IgsCode.Tell)
                {
                    HandleIncomingChatMessage(line);
                    weAreHandlingAnInterruptMessage = true;
                    continue;
                }
                if (code == IgsCode.Shout)
                {
                    HandleIncomingShoutMessage(line);
                    weAreHandlingAnInterruptMessage = true;
                    continue;
                }
                if (code == IgsCode.Move)
                {
                    HandleIncomingMove(igsLine);
                    weAreHandlingAnInterruptMessage = true;
                    continue;
                }
                
                

                // We cannot handle this generally - let's hand it off to whoever made the request for this information.
                lock (this._mutex)
                {
                    this._requestInProgress?.IncomingLines.Post(igsLine);
                }
            }
        }
        private void HandleIncomingMove(IgsLine igsLine)
        {
            string trim = igsLine.PureLine.Trim();
            if (trim.StartsWith("Game "))
            {
                string trim2 = trim.Substring("Game ".Length);
                int gameNumber = int.Parse(trim2.Substring(0, trim2.IndexOf(' ')));
                Game whatGame = this._gamesInProgressOnIgs.Find(gm => gm.ServerId == gameNumber);
                if (whatGame == null)
                {
                    whatGame = new Game();
                    whatGame.ServerId = gameNumber;
                    whatGame.Server = this;
                    this._gamesInProgressOnIgs.Add(whatGame);
                }
                this._incomingMovesAreForThisGame = whatGame;
            }
            else
            {
                Match match = regexMove.Match(trim);
                string moveIndex = match.Groups[1].Value;
                string mover = match.Groups[2].Value;
                string coordinates = match.Groups[3].Value;
                string captures = match.Groups[4].Value;
                Move move = Move.Create(mover == "B" ? Color.Black : Color.White,
                    Position.FromIgsCoordinates(coordinates));
                string[] captureSplit = captures.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string capture in captureSplit)
                {
                    move.Captures.Add(Position.FromIgsCoordinates(capture));
                }
                this._incomingMovesAreForThisGame.ForceMoveInHistory(int.Parse(moveIndex) + 1, move);
            }
        }
        private Regex regexMove = new Regex(@"([0-9]+)\((W|B)\): ([^ ]+)(.*)");


        /// <summary>
        /// Enqueues the <paramref name="command"/> to be sent over Telnet to the IGS SERVER,
        /// then asynchronously receives the entirety of the server's response to this command.
        /// </summary>
        /// <param name="command">The command to send over Telnet.</param>
        /// <returns></returns>
        private async Task<List<IgsLine>> MakeRequest(string command)
        {
            IgsRequest request = new IgsRequest(command);
            this._outgoingRequests.Enqueue(request);
            ExecuteRequestFromQueue();
            List<IgsLine> lines = await request.GetAllLines();
            lock (this._mutex)
            {
                Debug.Assert(this._requestInProgress == request);
                this._requestInProgress = null;
            }
            ExecuteRequestFromQueue();
            return lines;

        }
        private void MakeUnattendedRequest(string command)
        {
            IgsRequest request = new IgsRequest(command);
            request.Unattended = true;
            this._outgoingRequests.Enqueue(request);
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
            
            lock (this._mutex)
            {
                if (this._requestInProgress == null)
                {
                    IgsRequest dequeuedItem;
                    if (this._outgoingRequests.TryDequeue(out dequeuedItem))
                    {
                        if (!dequeuedItem.Unattended)
                        {
                            this._requestInProgress = dequeuedItem;
                        }
                        this._streamWriter.WriteLine(dequeuedItem.Command);
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




        // Interface requirements
        public override string ShortName => "IGS";
        public void RefreshBoard(Game game)
        {
            MakeUnattendedRequest("moves " + game.ServerId);
        }
    }
}
