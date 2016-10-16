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
            Console.Write("Pinging server: ");
            Console.WriteLine(connection.Hello());
        }
    }
}
