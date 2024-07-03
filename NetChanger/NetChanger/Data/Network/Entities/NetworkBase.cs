using NetChanger.Data.Network.ConnectionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetChanger.Data.Network.Entities
{
    public abstract class NetworkBase
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public abstract IConnectionMethod ConnectionMethod { get; }

        protected NetworkBase(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public abstract bool TryConnect();
    }
}
