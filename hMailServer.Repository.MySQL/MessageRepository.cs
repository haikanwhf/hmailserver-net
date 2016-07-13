using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
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
            message.Filename = Path.ChangeExtension(Path.Combine(_dataDirectory, Guid.NewGuid().ToString()), ".eml");

            using (var fileStream = File.Open(message.Filename, FileMode.CreateNew))
            {
                stream.CopyTo(fileStream);
            }

            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                sqlConnection.Open();
                
                var messageId = await sqlConnection.InsertAsync<long>(message);

                message.Id = (ulong) messageId;
            }
        }

        public async Task<Message> GetMessageToDeliverAsync()
        {
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                sqlConnection.Open();

                var messages =
                    await sqlConnection.QueryAsync<Message>("SELECT * FROM hm_messages WHERE messagetype = 1 LIMIT 1");

                var message = messages.SingleOrDefault();

                return message;
            }
        }
    }
}
