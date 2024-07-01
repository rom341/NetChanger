using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;

namespace NetChanger
{
    public class NetworkManager
    {
        private readonly string _pingAddress;

        public NetworkManager(string pingAddress)
        {
            _pingAddress = pingAddress;
        }

        public bool IsConnectedToInternet()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send(_pingAddress);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        public void ConnectToNetwork()
        {
            List<string> savedNetworks = GetSavedNetworks();

            foreach (string network in savedNetworks)
            {
                if (TryConnectToNetwork(network))
                {
                    // Wait for the network to connect
                    if (WaitForNetworkConnection())
                    {
                        // Exit the loop if connection is successful
                        if (IsConnectedToInternet())
                        {
                            Console.WriteLine($"Successfully connected to {network}");
                            return;
                        }
                    }
                }
            }

            Console.WriteLine("Unable to connect to any saved networks.");
        }

        private List<string> GetSavedNetworks()
        {
            List<string> networks = new List<string>();
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = "wlan show profiles",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process
            {
                StartInfo = processStartInfo
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Parse the output to get network names using a regex
            Regex profileRegex = new Regex(@":\s*(.+)$");
            foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains(":"))
                {
                    var match = profileRegex.Match(line);
                    if (match.Success)
                    {
                        var network = match.Groups[1].Value.Trim();
                        networks.Add(network);
                    }
                }
            }

            return networks;
        }

        private bool TryConnectToNetwork(string networkName)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = $"wlan connect name=\"{networkName}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process
            {
                StartInfo = processStartInfo
            };

            process.Start();
            process.WaitForExit();

            return process.ExitCode == 0;
        }

        private bool WaitForNetworkConnection()
        {
            int maxWaitTime = 30000; // Max wait time in milliseconds (30 seconds)
            int checkInterval = 1000; // Check interval in milliseconds (1 second)
            int waitedTime = 0;

            while (waitedTime < maxWaitTime)
            {
                if (IsNetworkConnected())
                {
                    return true;
                }

                Thread.Sleep(checkInterval);
                waitedTime += checkInterval;
            }

            return false;
        }

        private bool IsNetworkConnected()
        {
            foreach (var networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 &&
                    networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
