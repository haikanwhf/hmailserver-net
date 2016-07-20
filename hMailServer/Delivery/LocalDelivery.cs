using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Entities;
using hMailServer.Repository;

namespace hMailServer.Delivery
{
    public class LocalDelivery
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IFolderRepository _folderRepository;

        public LocalDelivery(IAccountRepository accountRepository, IMessageRepository messageRepository, IFolderRepository folderRepository)
        {
            _accountRepository = accountRepository;
            _messageRepository = messageRepository;
            _folderRepository = folderRepository;
        }

        public async Task DeliverAsync(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (message.Recipients == null)
                throw new ArgumentNullException(nameof(message.Recipients));

            var localRecipients = message.Recipients.Where(item => item.AccountId > 0);

            foreach (var localRecipient in localRecipients)
            {
                var localAccount = await _accountRepository.GetByIdAsync(localRecipient.AccountId);

                // TODO: Check quotas
                var inbox = await _folderRepository.GetInbox(localAccount.Id);

                var accountLevelMessage = await _messageRepository.CreateAccountLevelMessageAsync(message, localAccount, inbox);

                // TODO: Execute rules
                
                // TODO: Perform forwarding

                // TODO: Add Trace headers.

                // TODO: Etc

                accountLevelMessage.State = MessageState.Delivered;

                await _messageRepository.InsertAsync(accountLevelMessage);
            }
        }
    }
}
