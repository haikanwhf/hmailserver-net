using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Protocols.POP3;
using hMailServer.Entities;
using hMailServer.Repository;
using StructureMap;

namespace hMailServer.Protocols.POP3
{
    class Pop3ServerCommandHandler : IPop3ServerCommandHandler
    {
        private readonly IContainer _container;
        private List<Message> _messages;
        private Account _account;

        public Pop3ServerCommandHandler(IContainer container)
        {
            _container = container;
        }

        public async Task<Pop3CommandResult> HandleQuit()
        {
            if (_account != null && _messages != null)
            {
                var messagesToDelete = _messages.Where(message => message.Deleted);

                var messageRepository = _container.GetInstance<IMessageRepository>();

                foreach (var message in messagesToDelete)
                    await messageRepository.DeleteAsync(_account, message);
            }

            return Pop3CommandResult.CreateDefaultSuccess();
        }

        public Task<Pop3CommandResult> HandleStat()
        {
            var totalSize = _messages.Sum(f => f.Size);

            var responseMessage = string.Format("{0} {1}", _messages.Count, totalSize);

            return Task.Run(() => new Pop3CommandResult(true, responseMessage));
        }

        public Task<Pop3CommandResult> HandleList()
        {
            var totalSize = _messages.Where(msg => !msg.Deleted).Sum(msg => msg.Size);

            var builder = new StringBuilder();
            builder.AppendFormat("{0} messages ({1} octets)\r\n", _messages.Count, totalSize);

            for (int i = 0; i < _messages.Count; i++)
            {
                var message = _messages[i];

                if (message.Deleted)
                    continue;

                builder.AppendFormat("{0} {1}\r\n", i+1, message.Size);
            }

            builder.Append(".");

            return Task.Run(() => new Pop3CommandResult(true, builder.ToString()));
        }

        public async Task<Stream> HandleRetr(int messageNumber)
        {
            if (messageNumber > _messages.Count)
                return null;

            var message = _messages[messageNumber-1];

            if (message.Deleted)
                return null;

            var messageRepository = _container.GetInstance<IMessageRepository>();
            return messageRepository.GetMessageData(_account, message);
        }

        public Task<Pop3CommandResult> HandleDele(int messageNumber)
        {
            if (messageNumber > _messages.Count)
                return Task.Run(() => new Pop3CommandResult(false, "No such message"));

            var message = _messages[messageNumber - 1];

            if (message.Deleted)
            {
                return Task.Run(() => new Pop3CommandResult(false, "Message already deleted."));
            }

            message.Deleted = true;
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleNoop()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleRset()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleTop()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleUidl()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("\r\n");

            for (int i = 0; i < _messages.Count; i++)
            {
                var message = _messages[i];

                if (message.Deleted)
                    continue;

                builder.AppendFormat("{0} {1}\r\n", i + 1, message.Uid);
            }

            builder.Append(".");
            return Task.Run(() => new Pop3CommandResult(true, builder.ToString()));
        }

        public Task<Pop3CommandResult> HandleUser(string username)
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public async Task<Pop3CommandResult> HandlePass(string username, string password)
        {
            var accountRepository = _container.GetInstance<IAccountRepository>();

            _account = await accountRepository.ValidatePasswordAsync(username, password);

            if (_account == null)
                return new Pop3CommandResult(false, "Invalid username or password");

            var folderRepository = _container.GetInstance<IFolderRepository>();
            var inbox = await folderRepository.GetInbox(_account.Id);

            var messageRepository = _container.GetInstance<IMessageRepository>();
            _messages = await messageRepository.GetMessages(_account.Id, inbox.Id);

            return Pop3CommandResult.CreateDefaultSuccess();
        }

        public Task<Pop3CommandResult> HandleCapa()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }

        public Task<Pop3CommandResult> HandleStls()
        {
            return Pop3CommandResult.CreateDefaultSuccessTask();
        }
    }
}
