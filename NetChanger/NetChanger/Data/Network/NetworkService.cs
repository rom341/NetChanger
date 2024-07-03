using NetChanger.Data.Network.ConnectionMethods;
using NetChanger.Data.Network.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetChanger.Data.Network
{
    public class NetworkService
    {
        public List<List<NetworkBase>> GetSavedNetworks()
        {
            var savedNetworks = new List<List<NetworkBase>>();
            savedNetworks.Add(GetAvailableEthernetNetworks());
            savedNetworks.Add(GetAvailableWifiNetworks());
            savedNetworks.Add(GetAvailableStarlinkNetworks());
            savedNetworks.Add(GetAvailableGSMNetworks());

            return savedNetworks;
        }

        private List<NetworkBase> GetAvailableEthernetNetworks()
        {
            List<NetworkBase> ethernetNetworks = new List<NetworkBase>();

            // Пример получения доступных интерфейсов Ethernet
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



                // Парсинг вывода и добавление Ethernet интерфейсов
                Regex ethernetRegex = new Regex(@"(?:\S+\s+){3}(.+)", RegexOptions.Multiline);
                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Skip(2);//skip title
                foreach (var line in lines)
                {
                    var match = ethernetRegex.Match(line);
                    if (match.Success)
                    {
                        var name = match.Groups[1].Value.Trim();
                        var description = $"Ethernet interface: {name}";
                        ethernetNetworks.Add(new EthernetNetwork(name, description, new EthernetConnection()));
                    }
                }
            }

            return ethernetNetworks;
        }

        private List<NetworkBase> GetAvailableWifiNetworks()
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

                // Парсинг вывода и добавление Wi-Fi профилей
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

        private List<NetworkBase> GetAvailableGSMNetworks()
        {
            //TODO write code for GetAvailableGsmNetworks
            List<NetworkBase> gsmNetworks = new List<NetworkBase>();

            gsmNetworks.Add(new GsmNetwork("GSM Network 1", "Example GSM network", new GSMConnection()));

            return gsmNetworks;
        }


        private List<NetworkBase> GetAvailableStarlinkNetworks()
        {
            //TODO write code for GetAvailableStarlinkNetworks
            List<NetworkBase> starlinkNetworks = new List<NetworkBase>();

            starlinkNetworks.Add(new StarlinkNetwork("Starlink Network 1", "Example Starlink network", new StarlinkConnection()));

            return starlinkNetworks;
        }

    }
}
