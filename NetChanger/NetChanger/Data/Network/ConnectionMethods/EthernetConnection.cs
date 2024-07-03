using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChanger.Data.Network.ConnectionMethods
{
    public class EthernetConnection : IConnectionMethod
    {
        public override bool TryConnect(string NetworkName)
        {
            return ExecuteProcess("netsh", $"interface set interface \"{NetworkName}\" admin=enabled");
        }
    }
}
