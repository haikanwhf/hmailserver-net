using System.IO;

namespace hMailServer.Core.Protocols.SMTP
{
    public interface ISmtpServerCommandHandler
    {
        bool HandleHelo(string hostName);
        bool HandleMailFrom(string fromAddress);
        bool HandleRcptTo(string recipientAddress);
        bool HandleData(MemoryStream stream);
    }
}
