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

        public Task<SmtpCommandReply> HandleRset()
        {
            HeloHostname = null;
            EhloHostname = null;
            MailFrom = null;
            Recipients = new List<string>();
        
            return SmtpCommandReply.CreateDefault250SuccessTask();  
        }

        public Task<SmtpCommandReply> HandleHelo(string hostName)
        {
            HeloHostname = hostName;
            return SmtpCommandReply.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandReply> HandleEhlo(string hostName)
        {
            EhloHostname = hostName;
            return SmtpCommandReply.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandReply> HandleMailFrom(string fromAddress)
        {
            MailFrom = fromAddress;
            return SmtpCommandReply.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandReply> HandleRcptTo(string recipientAddress)
        {
            Recipients.Add(recipientAddress);
            return SmtpCommandReply.CreateDefault250SuccessTask();
        }

        public Task<SmtpCommandReply> HandleData(Stream stream)
        {
            stream.CopyTo(Body);
            Body.Seek(0, SeekOrigin.Begin);

            return SmtpCommandReply.CreateDefault250SuccessTask();
        }
    }
}
