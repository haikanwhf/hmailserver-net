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
        Rset,
        Helo,
        Ehlo,
        StartTls,
        MailFrom,
        RcptTo,
        Data,
        Quit
    }
}
