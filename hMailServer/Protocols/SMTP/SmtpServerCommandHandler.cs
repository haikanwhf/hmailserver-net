using System;
using System.IO;
using hMailServer.Core.Protocols.SMTP;

namespace hMailServer.Protocols.SMTP
{
    public class SmtpServerCommandHandler : ISmtpServerCommandHandler
    {
        private readonly SmtpServerSessionState _state = new SmtpServerSessionState();

        public SmtpCommandResult HandleRset()
        {
            _state.HandleRset();
            return SmtpCommandResult.Default250Success();
        }

        public SmtpCommandResult HandleHelo(string hostName)
        {
            return SmtpCommandResult.Default250Success();
        }

        public SmtpCommandResult HandleEhlo(string hostName)
        {
            return SmtpCommandResult.Default250Success();
        }

        public SmtpCommandResult HandleMailFrom(string fromAddress)
        {
            _state.FromAddress = fromAddress;
            return SmtpCommandResult.Default250Success();
        }

        public SmtpCommandResult HandleRcptTo(string recipientAddress)
        {
            _state.Recipients.Add(recipientAddress);
            return SmtpCommandResult.Default250Success();
        }

        public SmtpCommandResult HandleData(Stream stream)
        {
            string fileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            using (var fileStream = File.Open(fileName, FileMode.CreateNew))
            {
                stream.CopyTo(fileStream);
            }

            return SmtpCommandResult.Default250Success();
        }
    }
}
