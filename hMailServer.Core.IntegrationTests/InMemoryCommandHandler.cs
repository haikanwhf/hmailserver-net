using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hMailServer.Core.Protocols.SMTP;

namespace hMailServer.Core.IntegrationTests
{
    public class InMemoryCommandHandler : ISmtpServerCommandHandler
    {
        public string HeloHostname { get; set; }
        public string EhloHostname { get; set; }
        public string MailFrom { get; set; }
        public List<string> Recipients { get; set; } = new List<string>();
        public Stream Body;

        public SmtpCommandResult HandleRset()
        {
            HeloHostname = null;
            EhloHostname = null;
            MailFrom = null;
            Recipients = new List<string>();
        
            return SmtpCommandResult.Default250Success();  
        }

        public SmtpCommandResult HandleHelo(string hostName)
        {
            HeloHostname = hostName;
            return SmtpCommandResult.Default250Success();
        }

        public SmtpCommandResult HandleEhlo(string hostName)
        {
            EhloHostname = hostName;
            return SmtpCommandResult.Default250Success();
        }

        public SmtpCommandResult HandleMailFrom(string fromAddress)
        {
            MailFrom = fromAddress;
            return SmtpCommandResult.Default250Success();
        }

        public SmtpCommandResult HandleRcptTo(string recipientAddress)
        {
            Recipients.Add(recipientAddress);
            return SmtpCommandResult.Default250Success();
        }

        public SmtpCommandResult HandleData(Stream stream)
        {
            Body = stream;

            return SmtpCommandResult.Default250Success();
        }
    }
}
