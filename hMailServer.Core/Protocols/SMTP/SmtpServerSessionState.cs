using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.SMTP
{
    class SmtpServerSessionState
    {
        public bool HasHelo { get; set; }
        public bool HasMailFrom { get; set; }
        public bool HasRcptTo { get; set; }

        public void Reset()
        {
            HasHelo = false;
            HasMailFrom = false;
            HasRcptTo = false;
        }

        public bool IsCommandValid(SmtpCommand command)
        {
            switch (command)
            {
                case SmtpCommand.Help:
                case SmtpCommand.Helo:
                case SmtpCommand.Ehlo:
                case SmtpCommand.Rset:
                    return true;
                case SmtpCommand.StartTls:
                    return HasHelo && !HasMailFrom;
                case SmtpCommand.Mail:
                    return HasHelo && !HasMailFrom;
                case SmtpCommand.Rcpt:
                    return HasMailFrom;
                case SmtpCommand.Data:
                    return HasRcptTo;
                case SmtpCommand.Quit:
                    return true;
            }

            return false;
        }
    }
}
