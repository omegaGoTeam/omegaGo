using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Sockets.Plugin;

namespace OmegaGo.Core.Online
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
        private TcpSocketClient client;
        private StreamWriter streamWriter;
        private StreamReader streamReader;
        private List<Game> gamesInProgressOnIgs;
        private BufferBlock<string> incomingLines = new BufferBlock<string>();

        public void EnsureConnected()
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
        public void SendRawText(string command)
        {
            this.streamWriter.WriteLine(command);
        }
        
        public override async Task<List<Game>> ListGamesInProgress()
        {
            
            this.gamesInProgressOnIgs = new List<Core.Game>();
            string line;
            bool atLeastOneGamePassed = false;
            this.streamWriter.WriteLine("games");
            while ((line = await GetIncomingLine()) != null) {
                IgsCode code = ExtractCodeFromLine(line);
                if (code == IgsCode.Prompt && atLeastOneGamePassed)
                {
                    return gamesInProgressOnIgs;
                }
                if (code == IgsCode.Games)
                {
                    atLeastOneGamePassed = true;
                    if (line.Contains("[##]")) {
                        // This is the example line.
                        continue;
                    }
                    this.gamesInProgressOnIgs.Add(CreateGameFromTelnetLine(line));
                }
            }
            return gamesInProgressOnIgs;
            

        }
        private async Task<string> GetIncomingLine()
        {
            return await incomingLines.ReceiveAsync();
        }
        public IgsCode ExtractCodeFromLine(string line)
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
        public override void Observe(Game game)
        {
            base.Observe(game);
        }

        public override string ShortName => "IGS";

        private async void HandleIncomingData(StreamReader sr)
        {
            string line = await sr.ReadLineAsync();
            line = line.Trim();
            OnLogEvent(line);
            incomingLines.Post(line);
            HandleIncomingData(sr);
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

        public Game CreateGameFromTelnetLine(string line)
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
            } catch (FormatException)
            {
                Debug.WriteLine(line);
                return new Core.Game();
            }

        }
    }
}
