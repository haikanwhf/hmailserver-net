using System.Collections.Generic;

namespace hMailServer.Protocols.SMTP
{
    public class SmtpServerSessionState
    {
        public string FromAddress { get; set; }
        public List<string> Recipients = new List<string>();

        public void HandleRset()
        {
            FromAddress = null;
            Recipients.Clear();
        }
    }
}
