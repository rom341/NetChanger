using NetChanger.Data.Network.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetChanger.Data.Network
{
    public class NetworkConnectionOperator
    {
        private readonly string _pingAddress;
        private readonly NetworkService _networkService;

        public NetworkConnectionOperator(string pingAddress, NetworkService networkService)
        {
            this._pingAddress = pingAddress;
            this._networkService = networkService;
        }

        public void ChangeNetworkToOneThatWorks()
        {
            List<List<NetworkBase>> savedNetworks = _networkService.GetSavedNetworks();

            foreach (var networks in savedNetworks)
            {
                foreach (var network in networks)
                {
                    if (network.TryConnect())
                    {
                        // Wait for the network to connect
                        if (WaitForInternetConnection())
                        {
                            Console.WriteLine($"Successfully connected to {network.Name}");
                            return;
                        }
                    }
                }
            }

            Console.WriteLine("Unable to connect to any saved networks.");
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

        private bool WaitForInternetConnection()
        {
            const int maxAttempts = 20;
            const int retryIntervalMs = 200;

            for (int i = 0; i < maxAttempts; i++)
            {
                Thread.Sleep(retryIntervalMs);

                if (IsConnectedToInternet())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
