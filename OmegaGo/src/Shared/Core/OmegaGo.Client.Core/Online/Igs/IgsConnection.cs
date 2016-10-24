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
            this.gamesInProgressOnIgs = new List<Core.Game>();
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
                return new Core.Game();
            }

        }
        public override void StartObserving(Game game)
        {
            if (gamesBeingObserved.Contains(game))
            {
                // We are already observing this game.
                return; 
            }
            gamesBeingObserved.Add(game);
            streamWriter.WriteLine("observe " + game.ServerId);
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
                IgsLine igsLine = new Igs.IgsLine(code, line);
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
                
                

                // We cannot handle this generally - let's hand it off to whoever made the request for this information.
                lock (this.mutex)
                {
                    this.requestInProgress?.IncomingLines.Post(igsLine);
                }
            }
        }

        private void HandleIncomingChatMessage(string line)
        {
            OnIncomingChatMessage(line);
        }

        private async Task<List<IgsLine>> MakeRequest(string sayWhat)
        {
            IgsRequest request = new Igs.IgsRequest(sayWhat);
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


        public event Action<string> IncomingChatMessage;
        protected void OnIncomingChatMessage(string line)
        {
            IncomingChatMessage?.Invoke(line);
        }
        public event Action Beep;
        protected void OnBeep()
        {
            Beep?.Invoke();
        }

        // Interface requirements
        public override string ShortName => "IGS";
    }
    public class IgsRequest
    {
        public string Command;
        public IgsRequest(string command) { Command = command; }
        public BufferBlock<IgsLine> IncomingLines = new BufferBlock<IgsLine>();

        public async Task<List<IgsLine>> GetAllLines()
        {
            List<IgsLine> lines = new List<Igs.IgsLine>();
            while (true)
            {
                IgsLine line = await IncomingLines.ReceiveAsync();
                if (line.Code == IgsCode.Prompt)
                {
                    break;
                }
                lines.Add(line);
            }
            return lines;
        }
    }
    public class IgsLine
    {
        public IgsCode Code;
        public string EntireLine;
        public IgsLine(IgsCode code, string line)
        {
            Code = code;
            EntireLine = line;
        }
    }
}
