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
        private TcpSocketClient client;
        private StreamWriter streamWriter;

        private void EnsureConnected()
        {
            if (client != null) return;
            client = new TcpSocketClient();
            client.ConnectAsync("igs.joyjoy.net", 6969).Wait();
            this.streamWriter = new StreamWriter(this.client.WriteStream);
            StreamReader sr = new StreamReader(client.ReadStream);
            this.streamWriter.WriteLine("guest");
            CopyInput(sr);
        }

        private async void CopyInput(StreamReader sr)
        {
            string line = await sr.ReadLineAsync();
            OnLogEvent(line);
            CopyInput(sr);
        }

        public override string Hello()
        {
            EnsureConnected();
            streamWriter.WriteLine("help");
            return "DONE";
        }

        public override bool Login(string username, string password)
        {
            if (username == null) throw new ArgumentNullException(nameof(username));
            if (password == null) throw new ArgumentNullException(nameof(password));
            EnsureConnected();
            return false;
        }
    }
}
