using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core.Logging
{
    public class LogEvent
    {
        public IPEndPoint RemoteEndpoint { get; set; }
        public string SessionId { get; set; }
        public string Message { get; set; }
    }
}
