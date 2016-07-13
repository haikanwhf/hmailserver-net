using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using hMailServer.Core.Protocols.SMTP;
using hMailServer.Entities;
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
            var accountRepository = _container.GetInstance<IAccountRepository>();
            var account = accountRepository.GetByNameAsync(fromAddress).Result;

            bool isLocalAccount = account != null;

            _state.FromAddress = fromAddress;
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleRcptTo(string recipientAddress)
        {
            _state.Recipients.Add(recipientAddress);
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

        public async Task<SmtpCommandResult> HandleData(Stream stream)
        {
            var recipients = new List<Recipient>();

            foreach (var recipient in _state.Recipients)
            {
                recipients.Add(new Recipient()
                {
                    Address = recipient,
                    OriginalAddress = recipient,
                });
            }

            var message = new Message
                {
                    From = _state.FromAddress,
                    State = MessageState.Delivering,
                    //Recipients = recipients,
                    Size = (ulong)stream.Length
                };

            var messageRepository = _container.GetInstance<IMessageRepository>();
            await messageRepository.InsertAsync(message, stream);

            return SmtpCommandResult.CreateDefault250Success();
        }
    }
}
