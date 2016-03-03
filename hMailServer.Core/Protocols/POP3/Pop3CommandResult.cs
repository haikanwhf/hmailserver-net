using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hMailServer.Core.Protocols.POP3
{
    public class Pop3CommandResult
    {
        public bool Success { get; private set; }
        public string Message { get; private set; }

        public Pop3CommandResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
