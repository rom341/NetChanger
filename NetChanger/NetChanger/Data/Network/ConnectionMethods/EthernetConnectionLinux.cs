using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChanger.Data.Network.ConnectionMethods
{
    public class EthernetConnectionLinux : IConnectionMethod
    {
        public override bool TryConnect(string NetworkName)
        {
            return ExecuteProcess("nmcli", $"device connect {NetworkName}");
        }
    }
}
