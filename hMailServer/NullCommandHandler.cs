using System.Collections.Generic;
using System.IO;

using hMailServer.Core.Protocols.SMTP;

namespace hMailServer
{
    public class NullCommandHandler : ISmtpServerCommandHandler
    {
        private readonly List<string> _recipientAddress = new List<string>();

        public bool HandleHelo(string hostName)
        {
            return true;
        }

        public bool HandleMailFrom(string fromAddress)
        {
            // Validate blob exists in blob storage
            return true;
        }

        public bool HandleRcptTo(string recipientAddress)
        {
            _recipientAddress.Add(recipientAddress);
            return true;
        }

        public bool HandleData(MemoryStream stream)
        {
            return true;
        }
    }
}