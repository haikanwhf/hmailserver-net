using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.SMTP
{
    enum SmtpCommand
    {
        Unknown,
        Helo,
        Ehlo,
        MailFrom,
        RcptTo,
        Data,
        Quit
    }
}
