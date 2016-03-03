using System;
using System.Security.Cryptography.X509Certificates;

namespace hMailServer.Core.Protocols.IMAP
{
    public class ImapServerSessionConfiguration
    {
        public X509Certificate2 SslCertificate { get; set; }

        public TimeSpan CommandTimeout { get; set; }

        public ImapServerSessionConfiguration()
        {
            CommandTimeout = TimeSpan.FromMinutes(5);
        }
    }
}
