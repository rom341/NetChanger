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

            Console.WriteLine($"Ping address: {pingAddress}");
            Console.WriteLine($"Check interval: {checkInterval}ms");

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
                .SetBasePath(Directory.GetCurrentDirectory());

            string configFile = Path.Combine("Data", "appsettings.json");
            if (File.Exists(configFile))
            {
                builder.AddJsonFile(configFile, optional: false, reloadOnChange: true);
            }
            else
            {
                Console.WriteLine("Configuration file not found. Using default values.");
                var defaultSettings = new Dictionary<string, string>
                {
                    { "PingAddress", "8.8.8.8" },
                    { "CheckInterval", "3000" }
                };
                builder.AddInMemoryCollection(defaultSettings);
            }

            return builder.Build();
        }
    }
}
