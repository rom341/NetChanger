using NetChanger.Data.Network.ConnectionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChanger.Data.Network.Entities
{
    public class WifiNetwork : NetworkBase
    {
        public override IConnectionMethod ConnectionMethod { get; }

        public WifiNetwork(string name, string description, IConnectionMethod connectionMethod)
            : base(name, description)
        {
            ConnectionMethod = connectionMethod;
        }

        public override bool TryConnect()
        {
            return ConnectionMethod.TryConnect(Name);
        }
    }
}
