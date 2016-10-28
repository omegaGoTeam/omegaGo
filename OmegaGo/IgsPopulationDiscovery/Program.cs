using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Igs;

namespace IgsPopulationDiscovery
{
    class Program
    {
        static void Main(string[] args)
        {
            IgsConnection igs = new IgsConnection();
            igs.Login("OmegaGo1", "123456789");
            Console.Title = "IGS Population Discovery";

            Console.WriteLine("Press ENTER when you think login has been accomplished...");
            Console.ReadLine();



            while (true)
            {
                var users = igs.ListOnlinePlayers().Result;
                int count = users.Count;
                TryAppendCount(count);
                Console.WriteLine(count + " players are now online.");
                Thread.Sleep(60*1000);
            }
        }

        private static void TryAppendCount(int count)
        {
            try
            {
                System.IO.File.AppendAllLines("igsUserCount.csv",
                    new string[] {DateTime.Now.ToShortTimeString() + ";" + count});
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Failed to append.");
            }
        }
    }
}
