using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace NetChanger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Load configuration
            var config = LoadConfiguration();
            string pingAddress = config["PingAddress"];
            int checkInterval = int.Parse(config["CheckInterval"]);

            var networkManager = new NetworkManager(pingAddress);

            while (true)
            {
                if (!networkManager.IsConnectedToInternet())
                {
                    Console.WriteLine("No internet connection. Attempting to connect to another network...");
                    networkManager.ConnectToNetwork();
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
