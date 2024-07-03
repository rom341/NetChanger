using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using NetChanger.Data.Network;

namespace NetChanger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("For correct operation, the program must be run as administrator!");
            // Load configuration
            var config = LoadConfiguration();
            string pingAddress = config["PingAddress"];
            int checkInterval = int.Parse(config["CheckInterval"]);

            var networkOperator = new NetworkConnectionOperator(pingAddress, new NetworkService());

            while (true)
            {
                if (!networkOperator.IsConnectedToInternet())
                {
                    Console.WriteLine("No internet connection. Attempting to connect to another network...");
                    networkOperator.ChangeNetworkToOneThatWorks();
                }
                else
                {
                    Console.WriteLine("Internet connection is active.");
                }

                // Wait for the specified interval before checking again
                Thread.Sleep(checkInterval);
            }
        }

        static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }
    }
}
