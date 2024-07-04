using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChanger.Data.Network.ConnectionMethods
{
    public class WirelessConnectionLinux : IConnectionMethod
    {
        public override bool TryConnect(string NetworkName)
        {
            return ExecuteProcess("nmcli", $"device wifi connect {NetworkName}");
        }
    }
}
