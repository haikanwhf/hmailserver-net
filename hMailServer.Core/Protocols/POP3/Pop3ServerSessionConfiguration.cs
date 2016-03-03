using System;
using System.Security.Cryptography.X509Certificates;

namespace hMailServer.Core.Protocols.POP3
{
    public class Pop3ServerSessionConfiguration
    {
        public X509Certificate2 SslCertificate { get; set; }

        public TimeSpan CommandTimeout { get; set; }

        public Pop3ServerSessionConfiguration()
        {
            CommandTimeout = TimeSpan.FromMinutes(5);
        }
    }
}
