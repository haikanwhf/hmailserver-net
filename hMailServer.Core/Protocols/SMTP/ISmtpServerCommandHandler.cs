using System.IO;

namespace hMailServer.Core.Protocols.SMTP
{
    public interface ISmtpServerCommandHandler
    {
        SmtpCommandResult HandleRset();
        SmtpCommandResult HandleHelo(string hostName);
        SmtpCommandResult HandleEhlo(string hostName);
        SmtpCommandResult HandleMailFrom(string fromAddress);
        SmtpCommandResult HandleRcptTo(string recipientAddress);
        SmtpCommandResult HandleData(MemoryStream stream);
    }
}
