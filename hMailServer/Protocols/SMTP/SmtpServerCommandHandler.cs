using System;
using System.IO;
using System.Threading.Tasks;
using hMailServer.Core.Protocols.SMTP;
using hMailServer.Repository;
using StructureMap;

namespace hMailServer.Protocols.SMTP
{
    public class SmtpServerCommandHandler : ISmtpServerCommandHandler
    {
        private readonly SmtpServerSessionState _state = new SmtpServerSessionState();

        private readonly Container _container;

        public SmtpServerCommandHandler(Container container)
        {
            _container = container;
        }

        public Task<SmtpCommandResult> HandleRset()
        {
            _state.HandleRset();
            return SmtpCommandResult.Default250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleHelo(string hostName)
        {
            return SmtpCommandResult.Default250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleEhlo(string hostName)
        {
            return SmtpCommandResult.Default250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleMailFrom(string fromAddress)
        {
            var accountRepository = _container.GetInstance<IAccountRepository>();
            var account = accountRepository.GetByName(fromAddress).Result;

            bool isLocalAccount = account != null;

            _state.FromAddress = fromAddress;
            return SmtpCommandResult.Default250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleRcptTo(string recipientAddress)
        {
            _state.Recipients.Add(recipientAddress);
            return SmtpCommandResult.Default250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleData(Stream stream)
        {
            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            using (var fileStream = File.Open(fileName, FileMode.CreateNew))
            {
                stream.CopyTo(fileStream);
            }

            return SmtpCommandResult.Default250SuccessTask();
        }
    }
}
