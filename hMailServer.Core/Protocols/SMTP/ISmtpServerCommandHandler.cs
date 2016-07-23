using System.IO;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.SMTP
{
    public interface ISmtpServerCommandHandler
    {
        Task<SmtpCommandReply> HandleRset();
        Task<SmtpCommandReply> HandleHelo(string hostName);
        Task<SmtpCommandReply> HandleEhlo(string hostName);
        Task<SmtpCommandReply> HandleMailFrom(string fromAddress);
        Task<SmtpCommandReply> HandleRcptTo(string recipientAddress);
        Task<SmtpCommandReply> HandleData(Stream stream);
    }
}
