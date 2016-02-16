using System;
using System.Security.Cryptography.X509Certificates;

namespace hMailServer.Core.Protocols.SMTP
{
    public class SmtpServerSessionConfiguration
    {
        public X509Certificate2 SslCertificate { get; set; }

        public TimeSpan EnvelopeCommandTimeout { get; set; }
        public TimeSpan DataCommandTimeout { get; set; }

        public SmtpServerSessionConfiguration()
        {
            EnvelopeCommandTimeout = TimeSpan.FromMinutes(5);
            DataCommandTimeout = TimeSpan.FromMinutes(10);
        }
    }
}
