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
            float a = 0.5f;
            float b = 0.5f;
            float c = a + b;
            float d = c + 2;
            float e = d + b;
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
            Console.WriteLine("INCOMING: " + obj);
        }
    }
}
