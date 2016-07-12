using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.SMTP
{
    public class SmtpCommandResult
    {
        public int Code { get; private set; }
        public string Message { get; private set; }

        public SmtpCommandResult(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public bool IsPositive()
        {
            return Code >= 200 && Code <= 299;
        }

        public static Task<SmtpCommandResult> Default250SuccessTask()
        {
            return Task.Run(() => new SmtpCommandResult(250, "Ok."));
        }
    }
}
