using NetChanger.Data.Network.ConnectionMethods;
using NetChanger.Data.Network.Entities;
using NetChanger.Data.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetChanger.Data.Network
{
    public class NetworkService
    {
        public List<List<NetworkBase>> GetSavedNetworks()
        {
            var savedNetworks = new List<List<NetworkBase>>();

            if (OSPlatformHelper.IsWindows())
            {
                savedNetworks.Add(GetAvailableNetworksWindows());
                savedNetworks.Add(GetAvailableWirelessNetworksWindows());
            }
            else if (OSPlatformHelper.IsLinux())
            {
                savedNetworks.Add(GetAvailableNetworksLinux());
                savedNetworks.Add(GetAvailableWirelessNetworksLinux());
            }

            return savedNetworks;
        }

        private List<NetworkBase> GetAvailableNetworksWindows()
        {
            List<NetworkBase> ethernetNetworks = new List<NetworkBase>();

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = "interface show interface",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(psi);
            if (process != null)
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Regex ethernetRegex = new Regex(@"(?:\S+\s+){3}(.+)", RegexOptions.Multiline);
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(2); //skip title
                foreach (var line in lines)
                {
                    var match = ethernetRegex.Match(line);
                    if (match.Success)
                    {
                        var name = match.Groups[1].Value.Trim();
                        var description = $"Ethernet interface: {name}";
                        ethernetNetworks.Add(new EthernetNetwork(name, description, new EthernetConnectionWindows()));
                    }
                }
            }

            return ethernetNetworks;
        }

        private List<NetworkBase> GetAvailableWirelessNetworksWindows()
        {
            List<NetworkBase> wifiNetworks = new List<NetworkBase>();

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "netsh",
                Arguments = "wlan show profiles",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(psi);
            if (process != null)
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Regex profileRegex = new Regex(@":\s*(.+)$");
                foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (line.Contains(":"))
                    {
                        var match = profileRegex.Match(line);
                        if (match.Success)
                        {
                            var network = match.Groups[1].Value.Trim();
                            wifiNetworks.Add(new WifiNetwork(network, $"Wifi profile: {network}", new WifiConnection()));
                        }
                    }
                }
            }

            return wifiNetworks;
        }

        private List<NetworkBase> GetAvailableNetworksLinux()
        {
            List<NetworkBase> ethernetNetworks = new List<NetworkBase>();

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "nmcli",
                Arguments = "device status",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(psi);
            if (process != null)
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1); //skip title

                Regex ethernetRegex = new Regex(@"^\s*(\w+)\s+", RegexOptions.Multiline);
                foreach (string line in lines)
                {
                    var match = ethernetRegex.Match(line);
                    if (match.Success)
                    {
                        var name = match.Groups[1].Value.Trim();
                        var description = $"Ethernet interface: {match.Groups[2].Value.Trim()}";
                        ethernetNetworks.Add(new EthernetNetwork(name, description, new EthernetConnectionLinux()));
                    }
                }
            }

            return ethernetNetworks;
        }

        private List<NetworkBase> GetAvailableWirelessNetworksLinux()
        {
            List<NetworkBase> wifiNetworks = new List<NetworkBase>();

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "nmcli",
                Arguments = "device wifi list",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(psi);
            if (process != null)
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(1); //skip title

                Regex wifiRegex = new Regex(@"^\s*(\w+)\s+", RegexOptions.Multiline);
                foreach (string line in lines)
                {
                    var match = wifiRegex.Match(line);
                    if (match.Success)
                    {
                        var ssid = match.Groups[1].Value.Trim();
                        var description = $"Wifi SSID: {ssid}";
                        wifiNetworks.Add(new WifiNetwork(ssid, description, new WirelessConnectionLinux()));
                    }
                }
            }

            return wifiNetworks;
        }
    }
}
