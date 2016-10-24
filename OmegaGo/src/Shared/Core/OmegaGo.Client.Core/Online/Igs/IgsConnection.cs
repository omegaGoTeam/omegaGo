using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OmegaGo.Core.Extensions;
using Sockets.Plugin;

namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// Represents a connection established with the IGS server. This may not necessarily be a persistent TCP connection, but it retains information
    /// about which user is logged in.
    /// 
    /// http://web.archive.org/web/20050310114628/nngs.cosmic.org/help.html
    /// 
    /// </summary>
    /// <seealso cref="OmegaGo.Core.Online.ServerConnection" />
    public class IgsConnection : ServerConnection
    {
        // TODO switch prompt mode when necessary

        // Internal TCP connection objects
        private TcpSocketClient client;
        private StreamWriter streamWriter;
        private StreamReader streamReader;

        // Status
        private List<Game> gamesInProgressOnIgs;
        private List<Game> gamesBeingObserved = new List<Game>();


        public void SendRawText(string command)
        {
            EnsureConnected();
            this.streamWriter.WriteLine(command);
        }
        public override bool Login(string username, string password)
        {
            if (username == null) throw new ArgumentNullException(nameof(username));
            if (password == null) throw new ArgumentNullException(nameof(password));
            EnsureConnected();
            streamWriter.WriteLine("login");
            streamWriter.WriteLine(username);
            streamWriter.WriteLine(password);
            return true;
        }
        public override async Task<List<Game>> ListGamesInProgress()
        {
            EnsureConnected();
            this.gamesInProgressOnIgs = new List<Game>();
            List<IgsLine> lines = await MakeRequest("games");
            foreach(IgsLine line in lines)
            {
                if (line.Code == IgsCode.Games)
                {
                    if (line.EntireLine.Contains("[##]"))
                    {
                        // This is the example line.
                        continue;
                    }
                    this.gamesInProgressOnIgs.Add(CreateGameFromTelnetLine(line.EntireLine));
                }
            }
            return gamesInProgressOnIgs;
        }
        private Game CreateGameFromTelnetLine(string line)
        {
            Regex regex = new Regex(@"7 \[ *([0-9]+)] *([^[]+) \[([^]]+)\] vs. *([^[]+) \[([^]]+)\] \( *([0-9]+) *([0-9]+) *([0-9]+) *([-0-9.]+) *([0-9]+) *([A-Z]*)\) *\( *([0-9]+)\)");
            // The regex means:
            /*
             * 1 - game id
             * 2 - white name
             * 3 - white rank
             * 4 - black name
             * 5 - black rank
             * 6 - number of moves played
             * 7 - board size
             * 8 - handicap stones
             * 9 - komi points
             * 10 - byoyomi period
             * 11 - flags
             * 12 - number of observers 
             */
            Match match = regex.Match(line);
            try
            {
                Game game = new Game()
                {
                    ServerId = match.Groups[1].Value.AsInteger(),
                    Server = this,
                    Players = new List<Player>()
                {
                    new Player(match.Groups[4].Value, match.Groups[5].Value),
                    new Player(match.Groups[2].Value, match.Groups[3].Value)
                },
                    NumberOfMovesPlayed = match.Groups[6].Value.AsInteger(),
                    BoardSize = match.Groups[7].Value.AsInteger(),
                    NumberOfHandicapStones = match.Groups[8].Value.AsInteger(),
                    KomiValue = match.Groups[9].Value.AsFloat(),
                    NumberOfObservers = match.Groups[12].Value.AsInteger()
                };
                return game;
            }
            catch (FormatException)
            {
                Debug.WriteLine(line);
                return new Game();
            }

        }
        public override async void StartObserving(Game game)
        {
            if (gamesBeingObserved.Contains(game))
            {
                // We are already observing this game.
                return; 
            }
            gamesBeingObserved.Add(game);
            await MakeRequest("observe " + game.ServerId);
        }
        public override void EndObserving(Game game)
        {
            if (!gamesBeingObserved.Contains(game))
            {
                throw new ArgumentException("The specified game is currently not being observed.", nameof(game));
            }
            gamesBeingObserved.Remove(game);
            streamWriter.WriteLine("observe " + game.ServerId);
        }
        /// <summary>
        /// Sends a private message to the specified user using the 'tell' feature of IGS.
        /// </summary>
        /// <param name="recipient">The recipient.</param>
        /// <param name="message">The message.</param>
        /// <returns>True if the message was delivered.</returns>
        public async Task<bool> Tell(string recipient, string message)
        {
            List<IgsLine> result = await MakeRequest("tell " + recipient + " " + message);
            return result.All(line => line.Code != IgsCode.Error);
        }

        // Internal synchronization management
        private System.Collections.Concurrent.ConcurrentQueue<IgsRequest> outgoingRequests = new System.Collections.Concurrent.ConcurrentQueue<IgsRequest>();
        private IgsRequest requestInProgress;
        private void EnsureConnected()
        {
            if (client != null) return;

            client = new TcpSocketClient();
            Task.Run(() => client.ConnectAsync("igs.joyjoy.net", 6969)).Wait();
            this.streamWriter = new StreamWriter(this.client.WriteStream);
            this.streamReader = new StreamReader(this.client.ReadStream);
            this.streamWriter.AutoFlush = true;
            this.streamWriter.WriteLine("guest");
            this.streamWriter.WriteLine("toggle client on");
            this.streamWriter.WriteLine("toggle quiet on");
            HandleIncomingData(this.streamReader);
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
        private object mutex = new object();
        
        private async void HandleIncomingData(StreamReader sr)
        {
            bool weAreHandlingAnInterruptMessage = false;
            while (true)
            {
                string line = await sr.ReadLineAsync();
                if (line == null)
                {
                    OnLogEvent("The connection has been terminated."); // TODO propagate this to UI
                    client = null;
                    return;
                }
                line = line.Trim();

                // IGS occasionally sends blank lines, I don't know why. They serve no reason.
                if (line == "") continue; 

                IgsCode code = ExtractCodeFromLine(line);
                IgsLine igsLine = new IgsLine(code, line);
                OnLogEvent(line);
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
                
                

                // We cannot handle this generally - let's hand it off to whoever made the request for this information.
                lock (this.mutex)
                {
                    this.requestInProgress?.IncomingLines.Post(igsLine);
                }
            }
        }


        /// <summary>
        /// Enqueues the <paramref name="command"/> to be sent over Telnet to the IGS SERVER,
        /// then asynchronously receives the entirety of the server's response to this command.
        /// </summary>
        /// <param name="command">The command to send over Telnet.</param>
        /// <returns></returns>
        private async Task<List<IgsLine>> MakeRequest(string command)
        {
            IgsRequest request = new IgsRequest(command);
            this.outgoingRequests.Enqueue(request);
            ExecuteRequestFromQueue();
            List<IgsLine> lines = await request.GetAllLines();
            lock (this.mutex)
            {
                Debug.Assert(this.requestInProgress == request);
                this.requestInProgress = null;
            }
            ExecuteRequestFromQueue();
            return lines;

        }
        /// <summary>
        /// This method is called whenever a new command request is enqueued to be sent to the IGS SERVER and also whenever
        /// a command request becomes completed. The method will determine whether the channel is currently free (i.e. no other command
        /// is being executed) and if so, if there are any command requests in the queue, the earliest one is dequeued and executed.
        /// </summary>
        private void ExecuteRequestFromQueue()
        {
            
            lock (this.mutex)
            {
                if (this.requestInProgress == null)
                {
                    IgsRequest dequeuedItem;
                    if (this.outgoingRequests.TryDequeue(out dequeuedItem))
                    {
                        this.requestInProgress = dequeuedItem;
                        this.streamWriter.WriteLine(dequeuedItem.Command);
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

        public async void RefreshBoard(Game game)
        {
            List<IgsLine> moves = await MakeRequest("moves " + game.ServerId);
            
        }
    }
}
