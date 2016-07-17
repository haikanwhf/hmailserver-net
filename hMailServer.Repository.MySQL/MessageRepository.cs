using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using hMailServer.Core;
using hMailServer.Entities;
using MySql.Data.MySqlClient;

namespace hMailServer.Repository.MySQL
{
    class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;
        private readonly string _dataDirectory;

        public MessageRepository(string connectionString, string dataDirectory)
        {
            _connectionString = connectionString;
            _dataDirectory = dataDirectory;
        }

        public async Task InsertAsync(Message message, Stream stream)
        {
            var messageFullPath = Path.Combine(_dataDirectory, Path.ChangeExtension(Guid.NewGuid().ToString(), ".eml"));

            message.Filename = Path.GetFileName(messageFullPath);

            using (var fileStream = File.Open(messageFullPath, FileMode.CreateNew))
            {
                stream.CopyTo(fileStream);
            }

            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                sqlConnection.Open();
                
                var messageId = await sqlConnection.InsertAsync<long>(message);

                message.Id = messageId;
            }

            foreach (var recipient in message.Recipients)
            {
                recipient.MessageId = message.Id;

                await InsertAsync(recipient);
            }
        }

        public async Task UpdateAsync(Message message)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                sqlConnection.Open();

                await sqlConnection.UpdateAsync(message);
            }
        }

        public async Task DeleteAsync(Message message)
        {
            if (message.AccountId > 0)
                throw new NotImplementedException();

            var filename = Path.Combine(_dataDirectory, message.Filename);

            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                sqlConnection.Open();

                await sqlConnection.DeleteAsync(message);
            }

            File.Delete(filename);
        }

        public async Task<Message> GetMessageToDeliverAsync()
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                sqlConnection.Open();

                // TODO: MessageNextTryTime should be included in SQL

                var messages =
                    await sqlConnection.QueryAsync<Message>("SELECT * FROM hm_messages WHERE messagetype = 1 AND messagelocked = 0 LIMIT 1");
                var message = messages.SingleOrDefault();

                if (message != null)
                {
                    var recipients = await sqlConnection.QueryAsync<Recipient>(
                                            "SELECT * FROM hm_messagerecipients where recipientmessageid = @recipientmessageid",
                                                new
                                                {
                                                    recipientmessageid = message.Id
                                                });

                    foreach (var recipient in recipients)
                        message.Recipients.Add(recipient);
                }

                return message;
            }
        }

        public Task<Message> CreateAccountLevelMessageAsync(Message message, Account account)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            var clonedMessage = message.Clone();
            clonedMessage.Id = 0;
            clonedMessage.AccountId = account.Id;
            clonedMessage.Filename = Path.ChangeExtension(Guid.NewGuid().ToString(), ".eml");

            var accountMessageDirectory = GetAccountMessageDirectory(account);

            var messageDirectory = Path.Combine(accountMessageDirectory, clonedMessage.Filename.Substring(0, 2));
            var messageFileFullPath = Path.Combine(messageDirectory, clonedMessage.Filename);

            // TODO: Should be possible to do this asynchronously.
            Directory.CreateDirectory(messageDirectory);
            File.Copy(Path.Combine(_dataDirectory, message.Filename), messageFileFullPath);

            return Task.FromResult(clonedMessage);
        }

        public async Task InsertAsync(Recipient messageRecipient)
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                sqlConnection.Open();

                var messageRecipientId = await sqlConnection.InsertAsync<long>(messageRecipient);

                messageRecipient.Id = messageRecipientId;
            }
        }

        private string GetAccountMessageDirectory(Account account)
        {
            var mailbox = EmailAddressParser.GetMailbox(account.Address);
            var domain = EmailAddressParser.GetDomain(account.Address);

            return Path.Combine(_dataDirectory, domain, mailbox);
        }
    }
}
