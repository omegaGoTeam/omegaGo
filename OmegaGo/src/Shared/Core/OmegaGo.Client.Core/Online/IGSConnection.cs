using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void EnsureConnected()
        {
            if (client != null) return;

            client = new TcpSocketClient();
            client.ConnectAsync("igs.joyjoy.net", 6969).Wait();
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
        /*
        public override async Task<IEnumerable<Game>> ListGamesInProgress()
        {
            
            //this.gamesInProgressOnIgs = new List<Core.Game>();
            //while
            //this.streamWriter.WriteLine("games");
            

        }*/
        public override void Observe(Game game)
        {
            base.Observe(game);
        }

        private async void HandleIncomingData(StreamReader sr)
        {
            string line = await sr.ReadLineAsync();
            line = line.Trim();
            OnLogEvent(line);
            if (line != "")
            {
                string[] split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string firstWord = split[0];
                int firstWordAsInteger;
                if (int.TryParse(firstWord, out firstWordAsInteger))
                {
                    IgsCode code = (IgsCode)firstWordAsInteger;

                }
            }
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
    }
}
