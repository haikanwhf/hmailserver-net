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
        public MemoryStream Body = new MemoryStream();

        public Task<SmtpCommandResult> HandleRset()
        {
            HeloHostname = null;
            EhloHostname = null;
            MailFrom = null;
            Recipients = new List<string>();
        
            return SmtpCommandResult.CreateDefault250SuccessTask();  
        }

        public Task<SmtpCommandResult> HandleHelo(string hostName)
        {
            HeloHostname = hostName;
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleEhlo(string hostName)
        {
            EhloHostname = hostName;
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleMailFrom(string fromAddress)
        {
            MailFrom = fromAddress;
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleRcptTo(string recipientAddress)
        {
            Recipients.Add(recipientAddress);
            return SmtpCommandResult.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandResult> HandleData(Stream stream)
        {
            stream.CopyTo(Body);
            Body.Seek(0, SeekOrigin.Begin);

            return SmtpCommandResult.CreateDefault250SuccessTask();
        }
    }
}
