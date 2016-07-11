using System.IO;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.SMTP
{
    public interface ISmtpServerCommandHandler
    {
        Task<SmtpCommandResult> HandleRset();
        Task<SmtpCommandResult> HandleHelo(string hostName);
        Task<SmtpCommandResult> HandleEhlo(string hostName);
        Task<SmtpCommandResult> HandleMailFrom(string fromAddress);
        Task<SmtpCommandResult> HandleRcptTo(string recipientAddress);
        Task<SmtpCommandResult> HandleData(Stream stream);
    }
}
