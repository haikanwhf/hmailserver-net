using System.Security.Cryptography.X509Certificates;

namespace hMailServer.Core.Protocols.SMTP
{
    public class SmtpServerSessionConfiguration
    {
        public X509Certificate2 SslCertificate { get; set; }
    }
}
