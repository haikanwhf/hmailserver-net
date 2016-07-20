using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace hMailServer.Core
{
    public class ServerConfiguration
    {
        public IPAddress IpAddress { get; set; } = IPAddress.Any;
        public int Port { get; set; } = 0;

        public X509Certificate2 SslCertificate { get; set; }

        public ILog Log { get; set; }
    }
}
