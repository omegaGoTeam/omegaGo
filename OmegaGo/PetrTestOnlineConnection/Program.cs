using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs;

namespace PetrTestOnlineConnection
{
    class Program
    {
        static void Main(string[] args)
        {
            //TryOGS();
            TryIgs();
        }

/*        private static void TryOGS()
        {
            throw new NotImplementedException();
        }
        */
        private static void TryIgs()
        {
            IgsConnection connection = new IgsConnection();
            connection.LogEvent += Connection_LogEvent;
            Console.Write("== CONNECTING ==");
            Console.WriteLine();
            connection.EnsureConnected();
            Console.WriteLine();
            Console.WriteLine("== LOGGING IN ==");
            connection.Login("OmegaGoBot", "123456789");
            Console.WriteLine("== VIEWING GAMES ==");
            List<Game> games = connection.ListGamesInProgress().Result;
            Console.WriteLine($"MANY GAMES LOADED: {games.Count} IN TOTAL");
            Console.ReadKey();
        }

        private static void Connection_LogEvent(string obj)
        {
            Console.WriteLine("INCOMING: " + obj);
        }
    }
}
