using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online;

namespace PetrTestOnlineConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            IgsConnection connection = new IgsConnection();
            connection.LogEvent += Connection_LogEvent;
            Console.Write("Connecting: ");
            connection.EnsureConnected();
            Console.ReadKey();
            connection.Login("OmegaGoBot", "123456789");
            Console.ReadKey();
            connection.SendRawText("games");
            Console.ReadKey();
        }

        private static void Connection_LogEvent(string obj)
        {
            Console.WriteLine(obj);
        }
    }
}
