using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OmegaGo.Core.Online;
using OmegaGo.Core.Online.Igs;

namespace IgsPopulationDiscovery
{
    class Program
    {
        static void Main()
        {
            AsyncMain().Wait();
        }
        public static async Task AsyncMain()
        {
            IgsConnection igs = Connections.Pandanet;
            await igs.ConnectAsync();
            await igs.LoginAsync("OmegaGo1", "123456789");
            Console.Title = "IGS Population Discovery";
            Console.WriteLine("Login accomplished.");

            while (true)
            {
                try
                {
                    var users = igs.ListOnlinePlayersAsync().Result;
                    int count = users.Count;
                    TryAppendCount(count);
                    Console.WriteLine(count + " players are now online.");
                    Thread.Sleep(60 * 1000);
                }
                catch
                {
                    tryAgain:
                    try
                    {
                        Console.WriteLine("Connection lost. Attempting to reestablish...");
                        await igs.ConnectAsync();
                        await igs.LoginAsync("OmegaGo1", "123456789");
                        Thread.Sleep(2 * 1000);
                    } catch
                    {
                        Console.WriteLine("Failed again.");
                        Thread.Sleep(5 * 1000);
                        goto tryAgain;
                    }
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static void TryAppendCount(int count)
        {
            try
            {
                System.IO.File.AppendAllLines("igsUserCount.csv",
                    new[] {DateTime.Now.ToShortTimeString() + ";" + count});
            }
            catch
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Failed to append.");
            }
        }
    }
}
