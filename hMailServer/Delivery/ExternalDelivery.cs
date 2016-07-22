using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core;
using hMailServer.Core.Dns;
using hMailServer.Entities;
using hMailServer.Repository;

namespace hMailServer.Delivery
{
    public class ExternalDelivery
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IDnsClient _dnsClient;

        public ExternalDelivery(IMessageRepository messageRepository, IFolderRepository folderRepository, IDnsClient dnsClient)
        {
            _messageRepository = messageRepository;
            _folderRepository = folderRepository;
            _dnsClient = dnsClient;
        }

        public async Task DeliverAsync(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (message.Recipients == null)
                throw new ArgumentNullException(nameof(message.Recipients));

            var externalRecipients = message.Recipients.Where(item => item.AccountId == 0);

            var recipientsByDomain =
                externalRecipients.GroupBy(recipient => EmailAddressParser.GetDomain(recipient.Address));

            foreach (var domainWithRecipients in recipientsByDomain)
            {
                var ipAddresses = await _dnsClient.ResolveMxIpAddressesAsync(domainWithRecipients.Key);


                foreach (var ipAddress in ipAddresses)
                {
                    
                }
                // Lookup IP addresses for domainWithRecipients.Key

                // Connect to one at a time and try to deliver the message.
                // Result: 
                // List of remaining recipients with result code


            }
        }
    }
}
