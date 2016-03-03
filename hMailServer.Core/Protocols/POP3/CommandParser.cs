using System;
using System.Text.RegularExpressions;
using hMailServer.Core.Protocols.SMTP;

namespace hMailServer.Core.Protocols.POP3
{
    internal class CommandParser
    {
        public static Pop3Command ParseCommand(string command)
        {
            Pop3Command pop3Command;
            
            if (!Enum.TryParse(command, true, out pop3Command))
                pop3Command = Pop3Command.Unknown;

            return pop3Command;
        }
       
    }
}
