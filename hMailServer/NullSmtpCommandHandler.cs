using System.Collections.Generic;
using System.IO;

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

        public SmtpCommandResult HandleRset()
        {
            return new SmtpCommandResult(250, "Ok.");
        }

        public SmtpCommandResult HandleHelo(string hostName)
        {
            return new SmtpCommandResult(250, "Hello.");
        }


        public SmtpCommandResult HandleEhlo(string hostName)
        {
            return new SmtpCommandResult(250, "Ok.");
        }

        public SmtpCommandResult HandleMailFrom(string fromAddress)
        {


            return new SmtpCommandResult(250, "Ok.");
        }

        public SmtpCommandResult HandleRcptTo(string recipientAddress)
        {
            _recipientAddress.Add(recipientAddress);
            return new SmtpCommandResult(250, "Ok.");
        }

        public SmtpCommandResult HandleData(Stream stream)
        {
            return new SmtpCommandResult(250, "Ok.");
        }

    }
}