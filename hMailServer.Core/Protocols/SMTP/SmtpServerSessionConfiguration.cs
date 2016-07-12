using System;
using System.Security.Cryptography.X509Certificates;

namespace hMailServer.Core.Protocols.SMTP
{
    public class SmtpServerSessionConfiguration
    {
        public X509Certificate2 SslCertificate { get; set; }

        public TimeSpan EnvelopeCommandTimeout { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan DataCommandTimeout { get; set; } = TimeSpan.FromMinutes(10);

        public ConnectionSecurity ConnectionSecurity { get; set; }

        public string HostName { get; set; } = Environment.MachineName;
    }
}
