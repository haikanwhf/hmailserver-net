using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core
{
    public class ServerConfiguration
    {
        public IPAddress IpAddress = IPAddress.Parse("127.0.0.1");
        public int Port = 0;
    }
}
