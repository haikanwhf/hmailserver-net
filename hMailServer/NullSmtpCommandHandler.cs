using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using hMailServer.Core.Protocols.SMTP;
using hMailServer.Repository;

namespace hMailServer
{
    public class NullSmtpCommandHandler : ISmtpServerCommandHandler
    {
        private readonly List<string> _recipientAddress = new List<string>();

        private readonly IRepositoryFactory _repositoryFactory;

        public NullSmtpCommandHandler(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public Task<SmtpCommandResult> HandleRset()
        {
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleHelo(string hostName)
        {
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }
        
        public Task<SmtpCommandResult> HandleEhlo(string hostName)
        {
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleMailFrom(string fromAddress)
        {
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleRcptTo(string recipientAddress)
        {
            _recipientAddress.Add(recipientAddress);
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleData(Stream stream)
        {
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

    }
}